using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UNITTYPE { Warrior };

public class GameController : MonoBehaviour
{
    // TODO: Seperate unit list per player
    public HashSet<Unit> units;
    public GameObject UnitWarriorPrefab;
    public Dictionary<Unit, GameObject> unitToGameObjectMap;
    public bool AnimationIsPlaying = false;

    private void Awake()
    {
        units = new HashSet<Unit>();
        unitToGameObjectMap = new Dictionary<Unit, GameObject>();
    }

    private void Update()
    {
        // Hotkeys
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
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
    /// TODO: First check to see if there are any units that have enqueued moves. Do those moves.
    /// TODO: Now are any units waiting for orders? If so, halt EndTurn()
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

        foreach (Unit unit in units)
        {
            unit.RefreshMovement();
        }
        _ = StartCoroutine(DoAllUnitMoves());
    }

    public void SpawnUnitAt(UNITTYPE unitType, Hex h, Transform hexTransform)
    {
        Unit unit = new Unit("Unnamed", 100, 8, 2f, UNITTYPE.Warrior);
        unit.CanBuildCities = true;
        GameObject unitPrefab = GetPrefabForType(unitType);
        if (unitPrefab == null)
        {
            Debug.LogError("Unknown unit type. GameController::SpawnUnitAt()");
            return;
        }

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

    private GameObject GetPrefabForType(UNITTYPE unitType)
    {
        switch (unitType)
        {
            case UNITTYPE.Warrior:
                return UnitWarriorPrefab;
            default:
                return null;
        }
    }
}
