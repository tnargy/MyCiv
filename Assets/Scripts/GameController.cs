using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // TODO: Seperate unit list per player
    public int numPlayers = 1;
    public Player[] Players;
    public Player CurrentPlayer { get => Players[currentPlayerIndex]; }
    private int currentPlayerIndex = 0;

    public GameObject UnitWarriorPrefab;
    public GameObject VilliagePrefab;

    private HexMap hexMap;
    public bool AnimationIsPlaying = false;

    private void Update()
    {
        // Hotkeys for whole game
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndTurn();
        }
    }

    private void Start()
    {
        hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
        GeneratePlayers();
    }

    private void GeneratePlayers()
    {
        Players = new Player[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            Players[i] = new Player("Player1");
            SpawnPlayer(i == 0);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    public void DoAllUnitMoves()
    {
        if (CurrentPlayer.Units != null)
        {
            // Move all Units
            foreach (Unit unit in CurrentPlayer.Units)
            {
                StartCoroutine(DoUnitMoves(unit));
            }
        }
    }
    public IEnumerator DoUnitMoves(Unit unit)
    {
        while (unit.DoMove())
        {
            while (AnimationIsPlaying)
                yield return null;
        }
    }

    public void DoAllCityWork()
    {
        if (CurrentPlayer.Cities != null)
        {
            // Move all Units
            foreach (City city in CurrentPlayer.Cities)
            {
                StartCoroutine(DoCityTurn(city));
            }
        }
    }

    public IEnumerator DoCityTurn(City city)
    {
        city.DoTurn();
        yield return null;
    }

    /// <summary>
    /// First check to see if there are any units that have enqueued moves. Do those moves.
    /// Now are any units waiting for orders? If so, halt EndTurn()
    /// 
    /// TODO: Heal units that are resting
    /// Reset unit movements
    /// 
    /// TODO: Go to next player
    /// </summary>
    public void EndTurn()
    {
        foreach (Unit unit in CurrentPlayer.Units)
        {
            if (unit.UnitWaitingForOrders())
            {
                unit.SkipTurn = true;
                Camera.main.GetComponent<CameraMotion>().MoveToHex(unit.Hex);
                return;
            }
        }

        foreach (Unit unit in CurrentPlayer.Units)
        {
            unit.RefreshMovement();
        }

        DoAllUnitMoves();
        DoAllCityWork();
        currentPlayerIndex = (currentPlayerIndex + 1) % Players.Length;
    }

    public void SpawnPlayer(bool zoomCamera = true)
    {
        bool respawn = true;
        Hex spawnHex = hexMap.GetHexAt(0, 0);
        while (respawn)
        {
            respawn = false;
            spawnHex = hexMap.GetHexAt(Random.Range(2, hexMap.MapX - 2), Random.Range(2, hexMap.MapY - 2));
            Hex[] spawnArea = hexMap.GetHexesWithinRangeOf(spawnHex, 2);
            foreach (Hex h in spawnArea)
            {
                if (h.Elevation < hexMap.HeightFlat)
                    respawn = true;
            }
            if (spawnHex.Terrain != Hex.TERRAINTYPE.Plains || spawnHex.isHill)
                respawn = true;
        }
        Unit unit = new Unit("Warrior", 100, 8, 2f, Unit.UNITTYPE.Warrior)
        {
            CanBuildCities = true
        };

        SpawnUnitAt(unit, spawnHex);
        if (zoomCamera)
            Camera.main.GetComponent<CameraMotion>().MoveToHex(spawnHex);
    }

    public void SpawnUnitAt(Unit unit, Hex h)
    {
        GameObject unitPrefab = GetPrefabForType(unit.UnitType);
        if (unitPrefab == null)
        {
            Debug.LogError("Unknown unit type. GameController::SpawnUnitAt()");
            return;
        }
        Transform hexTransform = hexMap.GetGameObjectFromHex(h).transform;
        GameObject unitObj = Instantiate(
            unitPrefab,
            hexTransform.position,
            Quaternion.identity,
            hexTransform);

        unit.SetHex(h);
        unit.OnObjectMoved += unitObj.GetComponent<MapObjectView>().OnObjectMoved;
        CurrentPlayer.AddUnit(unit, unitObj);
    }

    private GameObject GetPrefabForType(Unit.UNITTYPE unitType)
    {
        switch (unitType)
        {
            case Unit.UNITTYPE.Warrior:
                return UnitWarriorPrefab;
            default:
                return null;
        }
    }

    public void AddCity(City city)
    {
        Transform hexTransform = hexMap.GetGameObjectFromHex(city.Hex).transform;
        Vector3 spawnLocation = hexTransform.position;
        if (city.Hex.isHill)   // Move city offset to account for hill
            spawnLocation.Set(hexTransform.position.x, hexTransform.position.y + 0.15f, hexTransform.position.z);

        GameObject cityObj = Instantiate(
            VilliagePrefab,
            spawnLocation,
            Quaternion.identity,
            hexTransform);
        cityObj.GetComponentInChildren<TextMeshProUGUI>().text = city.Name;

        CurrentPlayer.AddCity(city, cityObj);
    }
}
