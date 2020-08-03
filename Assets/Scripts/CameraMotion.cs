using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    HexBehavior[] hexes;

    // Start is called before the first frame update
    void Start()
    {
        hexes = GameObject.FindObjectsOfType<HexBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp the NORTH/SOUTH movement of camera
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            Mathf.Clamp(transform.position.z, 0f, HexMap.mapHeightLimit));

        // TODO: Code to click-and-drag camera
        //              WASD
        //              Zoom in/out

        CheckIfCameraMoved();
    }

    private void CheckIfCameraMoved()
    {
        if (transform.hasChanged)
        {
            foreach (HexBehavior hex in hexes)
            {
                hex.UpdatePosition();
            }
        }
    }

    public void PanToHex(Hex hex)
    {
        // TODO: Move camera to hex
    }
}
