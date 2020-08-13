using TMPro;
using UnityEngine;

public class UnitSelectionPanel : MonoBehaviour
{
    MouseController mouseController;
    public TextMeshProUGUI SelectedUnit;
    public TextMeshProUGUI Movement;
    public TextMeshProUGUI DebugString;
    public GameObject BuildCityAction;

    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Hotkey for Actions
        if (Input.GetKeyUp(KeyCode.B) && BuildCityAction.activeSelf)
        {
            // Build City
            BuildCity();
        }

        if (mouseController.SelectedUnit != null)
        {
            SelectedUnit.text = $"Selected Unit: {mouseController.SelectedUnit.Name}";
            Movement.text = $"Movement: {mouseController.SelectedUnit.MovementRemaining}/{mouseController.SelectedUnit.Movement}";
            Hex[] path = mouseController.SelectedUnit.GetHexPath();
            if (path != null)
                DebugString.text = $"Path Length: {path.Length - 1}";
            else
                DebugString.text = "";
            BuildCityAction.SetActive(mouseController.SelectedUnit.CanBuildCities
                && mouseController.SelectedUnit.Hex.City == null
                && mouseController.SelectedUnit.Hex.Terrain != Hex.TERRAINTYPE.Forest
                && mouseController.SelectedUnit.Hex.Terrain != Hex.TERRAINTYPE.Jungle);
        }
    }

    public void BuildCity()
    {
        mouseController.SelectedUnit.BuildCity();
    }
}
