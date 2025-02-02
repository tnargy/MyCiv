﻿using System.Collections.Generic;
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
        if (BuildingJob != null)
            currentProduction = PRODUCTION.BUSY;

        if (currentProduction == PRODUCTION.BUSY)
        {
            workLeft = BuildingJob.DoWork(productionPerTurn);
        }
    }

    public void Build(int index)
    {
        if (currentProduction != PRODUCTION.BUSY)
        {
            BuildingJob = BuildingJobsList[index];
            BuildingJob.TotalProductionNeeded -= workLeft;
        }

    }

    private void BuildList()
    {
        BuildingJobsList = new List<BuildingJob>();

        // Settler
        var job = new BuildingJob(Resources.Load<Sprite>("Icons/Unit.Warrior"),
                       "Settler",
                       2,
                       () =>    // OnCompleteFunc
                       {
                           Unit unit = new Unit("Settler", 100, 0, 2f, Unit.UNITTYPE.Settler)
                           {
                               CanBuildCities = true
                           };
                           GameObject.Find("GameController").GetComponent<GameController>().SpawnUnitAt(unit, Hex);
                           currentProduction = PRODUCTION.IDLE;
                           BuildingJob = null;
                       });
        BuildingJobsList.Add(job);
        
        // Warrior
        job = new BuildingJob(Resources.Load<Sprite>("Icons/Unit.Warrior"),
                       "Warrior",
                       5,
                       () =>    // OnCompleteFunc
                       {
                           Unit unit = new Unit("Warrior", 100, 8, 2f, Unit.UNITTYPE.Warrior);
                           GameObject.Find("GameController").GetComponent<GameController>().SpawnUnitAt(unit, Hex);
                           currentProduction = PRODUCTION.IDLE;
                           BuildingJob = null;
                       });
        BuildingJobsList.Add(job);


    }
}