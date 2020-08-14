using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // TODO: Seperate unit list per player
    private HashSet<Unit> units;
    private HashSet<City> cities;

    public GameObject UnitWarriorPrefab;
    public GameObject VilliagePrefab;

    private Dictionary<Unit, GameObject> unitToGameObjectMap;
    private Dictionary<City, GameObject> cityToGameObjectMap;
    public bool AnimationIsPlaying = false;
    private HexMap hexMap;


    private void Awake()
    {
        units = new HashSet<Unit>();
        unitToGameObjectMap = new Dictionary<Unit, GameObject>();

        cities = new HashSet<City>();
        cityToGameObjectMap = new Dictionary<City, GameObject>();

    }

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
        SpawnPlayer(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    IEnumerator DoAllUnitMoves()
    {
        if (units != null)
        {
            // Move all Units
            foreach (Unit unit in units)
            {
                yield return DoUnitMoves(unit);
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
        foreach (Unit unit in units)
        {
            if (unit.UnitWaitingForOrders())
            {
                Camera.main.GetComponent<CameraMotion>().MoveToHex(unit.Hex);
                return;
            }
        }

        _ = StartCoroutine(DoAllUnitMoves());

        foreach (Unit unit in units)
        {
            unit.RefreshMovement();
        }

        foreach (City city in cities)
        {
            city.DoTurn();
        }
    }

    public void SpawnPlayer(int playerIndex, bool zoomCamera = true)
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
        unitToGameObjectMap.Add(unit, unitObj);
        units.Add(unit);
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
        if (city.Hex.isHill)
            spawnLocation.Set(hexTransform.position.x, hexTransform.position.y + 0.15f, hexTransform.position.z);

        GameObject cityObj = Instantiate(
            VilliagePrefab,
            spawnLocation,
            Quaternion.identity,
            hexTransform);
        cityObj.GetComponentInChildren<TextMeshProUGUI>().text = city.Name;

        cityToGameObjectMap.Add(city, cityObj);
        cities.Add(city);
    }

    public void RemoveCity(City city)
    {
        cityToGameObjectMap.Remove(city);
        cities.Remove(city);
    }
}
