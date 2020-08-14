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
        throw new NotImplementedException();
    }
}
