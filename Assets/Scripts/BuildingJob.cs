using System;
using UnityEngine;

public class BuildingJob
{

    public float TotalProductionNeeded, CurrentProductinDone;

    public Sprite icon;
    public string name;

    public delegate void ProductionCompleteDelegate();
    public ProductionCompleteDelegate OnProductionComplete;

    public BuildingJob(Sprite icon,
                       string name,
                       float totalProductionNeeded,
                       float overflowProduction,
                       ProductionCompleteDelegate OnProductionComplete)
    {
        TotalProductionNeeded = totalProductionNeeded + overflowProduction;
        this.icon = icon ?? throw new ArgumentNullException(nameof(icon));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
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
