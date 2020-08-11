using TMPro;
using UnityEngine;

public class UnitSelectionPanel : MonoBehaviour
{
    MouseController mouseController;
    public TextMeshProUGUI SelectedUnit;
    public TextMeshProUGUI Movement;
    public TextMeshProUGUI PathLength;

    // Start is called before the first frame update
    void Start()
    {
        mouseController = FindObjectOfType<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseController.SelectedUnit != null)
        {
            SelectedUnit.text = $"Selected Unit: {mouseController.SelectedUnit.Name}";
            Movement.text = $"Movement: {mouseController.SelectedUnit.Movement}";
            Hex[] path = mouseController.SelectedUnit.GetHexPath();
            if (path != null)
                PathLength.text = $"Path Length: {path.Length - 1}";
            else 
                PathLength.text = "";
        }
    }
}
