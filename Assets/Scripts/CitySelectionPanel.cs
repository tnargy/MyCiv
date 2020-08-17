using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CitySelectionPanel : MonoBehaviour
{
    MouseController mouseController;
    public TextMeshProUGUI Name;
    public GameObject BuildList;
    public GameObject ProductionPanel;
    private GameObject buildItemPrefab;
    private int jobCount;

    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
        buildItemPrefab = (GameObject)Resources.Load("Prefabs/BuildItem");
        ProductionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseController.SelectedCity != null)
        {
            ProductionPanel.SetActive(false);
            City city = mouseController.SelectedCity;
            ProductionPanel.SetActive(city.currentProduction != City.PRODUCTION.IDLE);

            Name.text = city.Name;

            if (jobCount != city.BuildingJobsList.Count)
            {
                foreach (var item in city.BuildingJobsList)
                {
                    var buildItem = Instantiate(buildItemPrefab, BuildList.transform);
                    buildItem.GetComponent<Button>().onClick.AddListener(() => Production(city.BuildingJobsList.IndexOf(item)));
                    buildItem.transform.Find("Icon").GetComponent<Image>().sprite = item.Icon;
                    buildItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Name;
                    buildItem.transform.Find("Turns").GetComponent<TextMeshProUGUI>().text = $"(Turns: {(int)Mathf.Ceil(item.TotalProductionNeeded/city.productionPerTurn)})";
                }
                jobCount = city.BuildingJobsList.Count;
            }

            if (city.BuildingJob != null && city.currentProduction == City.PRODUCTION.BUSY)
            {
                ProductionPanel.SetActive(true);
                Transform content = ProductionPanel.transform.Find("Content").transform;
                content.Find("Icon").GetComponent<Image>().sprite = city.BuildingJob.Icon;
                content.Find("Production").GetComponent<TextMeshProUGUI>().text =
                    $"Current Item:\n" +
                    $"{city.BuildingJob.Name}\n" +
                    $"Production: {city.BuildingJob.CurrentProductinDone} / {city.BuildingJob.TotalProductionNeeded}\n" +
                    $"Turns: {(int)Mathf.Ceil(city.BuildingJob.TotalProductionNeeded / (city.BuildingJob.CurrentProductinDone == 0 ? 1 : city.BuildingJob.CurrentProductinDone))}";
            }
        }
    }

    public void Production(int index)
    {
        City city = mouseController.SelectedCity;
        city.Build(index);
    }
}
