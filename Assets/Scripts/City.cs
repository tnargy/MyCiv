using UnityEngine;

public class City : MapObject
{
    public enum PRODUCTION { IDLE, BUSY };
    public PRODUCTION currentProduction;
    BuildingJob buildingJob;
    float productionPerTurn = 9001;
    float workLeft;

    public City(Hex hex, string name)
    {
        this.Hex = hex;
        Name = name;
        currentProduction = PRODUCTION.IDLE;

        EXAMPLE();
    }

    void EXAMPLE()
    {
        currentProduction = PRODUCTION.BUSY;
        Unit unit = new Unit("Unnamed", 100, 8, 2f, Unit.UNITTYPE.Warrior);
        Sprite icon = Resources.Load<Sprite>("Icons/Unit.Warrior");
        buildingJob = new BuildingJob(icon,
                       "Warrior",
                       100,
                       workLeft,
                       () =>
                       {
                           GameObject.Find("GameController").GetComponent<GameController>().SpawnUnitAt(unit, Hex);
                           currentProduction = PRODUCTION.IDLE;
                       });
    }

    public void DoTurn()
    {
        if (currentProduction == PRODUCTION.BUSY)
        {
            workLeft = buildingJob.DoWork(productionPerTurn);
        }
    }
}