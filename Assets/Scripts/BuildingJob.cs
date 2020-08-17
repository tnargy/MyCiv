using System;
using UnityEngine;

public class BuildingJob
{

    public float TotalProductionNeeded, CurrentProductinDone;

    public Sprite Icon;
    public string Name;

    public delegate void ProductionCompleteDelegate();
    public ProductionCompleteDelegate OnProductionComplete;

    public BuildingJob(Sprite icon,
                       string name,
                       float totalProductionNeeded,
                       ProductionCompleteDelegate OnProductionComplete)
    {
        TotalProductionNeeded = totalProductionNeeded;
        Icon = icon;
        Name = name;
        this.OnProductionComplete = OnProductionComplete ?? throw new ArgumentNullException(nameof(OnProductionComplete));
    }

    public float DoWork(float rawProduction)
    {
        CurrentProductinDone += rawProduction;
        if (CurrentProductinDone >= TotalProductionNeeded)
        {
            OnProductionComplete();
        }

        return TotalProductionNeeded - CurrentProductinDone;
    }
}
