using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    HexBehavior[] hexes;
    private float moveSpeed = 10f;
    private Vector3 oldPosition;

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

        Vector3 translate = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical"));
        transform.Translate(translate * moveSpeed * Time.deltaTime, Space.World);

        CheckIfCameraMoved();
    }

    private void CheckIfCameraMoved()
    {
        if (transform.position != oldPosition)
        {
            foreach (HexBehavior hexB in hexes)
            {
                hexB.UpdatePosition();
            }
            oldPosition = transform.position;
        }
    }

    public void MoveToHex(Hex h)
    {
        h = h.HexMap.GetHexAt(h.Q + 1, h.R - 2);
        transform.position = h.HexMap.GetGameObjectFromHex(h).transform.position;
    }
}
