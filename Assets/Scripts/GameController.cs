using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum UNITTYPE { Warrior };

public class GameController: MonoBehaviour
{
    public HashSet<Unit> units;
    public GameObject UnitWarriorPrefab;
    public Dictionary<Unit, GameObject> unitToGameObjectMap;
    
    private void Start()
    {
        units = new HashSet<Unit>();
        unitToGameObjectMap = new Dictionary<Unit, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            {
                if (units != null)
                {
                    foreach (Unit unit in units)
                    {
                        while (unit.DoMove())
                        {
                            // Coroutine here for animation
                        } 
                    }

                    foreach (Unit unit in units)
                    {
                        unit.MovementRemaining = unit.Movement;
                    }
                }
            }
    }

    public void SpawnUnitAt(UNITTYPE unitType, Hex h, Transform hexTransform)
    {
        Unit unit = new Unit();
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

        units.Add(unit);
        unitToGameObjectMap.Add(unit, unitObj);
        unit.SetHex(h);
        unit.OnUnitMoved += unitObj.GetComponent<UnitView>().OnUnitMoved;
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
