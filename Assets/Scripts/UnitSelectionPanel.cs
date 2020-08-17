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
            Unit unit = mouseController.SelectedUnit;
            SelectedUnit.text = $"Selected Unit: {unit.Name}";
            Movement.text = $"Movement: {unit.MovementRemaining}/{unit.Movement}";
            Hex[] path = unit.GetHexPath();
            if (path != null)
                DebugString.text = $"Path Length: {path.Length - 1}";
            else
                DebugString.text = "";
            BuildCityAction.SetActive(unit.CanBuildCities
                && unit.Hex.City == null
                && unit.Hex.Terrain != Hex.TERRAINTYPE.Forest
                && unit.Hex.Terrain != Hex.TERRAINTYPE.Jungle);
        }
    }

    public void BuildCity()
    {
        // string cityName = GetName(GameObject.Find("Middle").GetComponent<Canvas>(), GameObject.Find("Middle").GetComponent<Rename>());
        mouseController.SelectedUnit.BuildCity("Tulsa");
    }

    private string GetName(Canvas canvas, Rename rename)
    {
        canvas.enabled = true;
        if (rename.input == rename.inputField.text && rename.input == "")
            return GetName(canvas, rename);
        return rename.input;
    }
}
