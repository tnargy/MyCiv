using UnityEngine;

public class HexBehavior : MonoBehaviour
{
    public Hex hex;
    public HexMap hexMap;

    public void UpdatePosition()
    {
        transform.position = hexMap.PositionFromCamera(hex);
    }
}
