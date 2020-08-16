using System;
using TMPro;
using UnityEngine;

public class CitySelectionPanel : MonoBehaviour
{
    MouseController mouseController;
    public TextMeshProUGUI Name;
    public GameObject ProductionPanel;

    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseController.SelectedCity != null)
        {
            City city = mouseController.SelectedCity;
            ProductionPanel.SetActive(city.currentProduction != City.PRODUCTION.IDLE);

            Name.text = city.Name;
        }
    }

    public void Production()
    {
        City city = mouseController.SelectedCity;
        city.currentProduction = City.PRODUCTION.BUSY;
        Unit unit = new Unit("Unnamed", 100, 8, 2f, Unit.UNITTYPE.Warrior);
        Sprite icon = Resources.Load<Sprite>("Icons/Unit.Warrior");
        city.buildingJob = new BuildingJob(icon,
                       "Warrior",
                       100,
                       city.workLeft,
                       () =>
                       {
                           GameObject.Find("GameController").GetComponent<GameController>().SpawnUnitAt(unit, city.Hex);
                           city.currentProduction = City.PRODUCTION.IDLE;
                       });
    }
}
