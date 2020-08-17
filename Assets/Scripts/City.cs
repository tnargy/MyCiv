using System.Collections.Generic;
using UnityEngine;

public class City : MapObject
{
    public enum PRODUCTION { IDLE, BUSY };
    public PRODUCTION currentProduction;
    public List<BuildingJob> BuildingJobsList;
    public  BuildingJob BuildingJob;
    public float workLeft;
    public float productionPerTurn = 1;

    public City(Hex hex, string name)
    {
        this.Hex = hex;
        Name = name;
        workLeft = 0;
        currentProduction = PRODUCTION.IDLE;
        BuildList();
    }

    public void DoTurn()
    {
        if (currentProduction == PRODUCTION.BUSY)
        {
            workLeft = BuildingJob.DoWork(productionPerTurn);
        }
    }

    public void Build(int index)
    {
        if (currentProduction != PRODUCTION.BUSY)
        {
            currentProduction = PRODUCTION.BUSY;
            BuildingJob = BuildingJobsList[index];
            BuildingJob.TotalProductionNeeded -= workLeft;
        }

    }

    private void BuildList()
    {
        BuildingJobsList = new List<BuildingJob>();

        // Warrior
        Sprite icon = Resources.Load<Sprite>("Icons/Unit.Warrior");
        var job = new BuildingJob(icon,
                       "Warrior",
                       5,
                       () =>
                       {
                           Unit unit = new Unit("Warrior", 100, 8, 2f, Unit.UNITTYPE.Warrior);
                           GameObject.Find("GameController").GetComponent<GameController>().SpawnUnitAt(unit, Hex);
                           currentProduction = PRODUCTION.IDLE;
                           BuildingJob = null;
                       });
        BuildingJobsList.Add(job);
    }
}