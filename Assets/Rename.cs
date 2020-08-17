using TMPro;
using UnityEngine;

public class Rename : MonoBehaviour
{
    public string input;
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void Save()
    {
        input = inputField.text;
        inputField.text = null;
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
