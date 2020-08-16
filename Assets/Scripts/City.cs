using UnityEngine;

public class City : MapObject
{
    public enum PRODUCTION { IDLE, BUSY };
    public PRODUCTION currentProduction;
    public BuildingJob buildingJob;
    public float workLeft;
    private float productionPerTurn = 9001;

    public City(Hex hex, string name)
    {
        this.Hex = hex;
        Name = name;
        currentProduction = PRODUCTION.IDLE;
    }

    public void DoTurn()
    {
        if (currentProduction == PRODUCTION.BUSY)
        {
            workLeft = buildingJob.DoWork(productionPerTurn);
        }
    }
}