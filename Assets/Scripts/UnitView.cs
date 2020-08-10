using UnityEngine;

public class UnitView : MonoBehaviour
{
    Vector3 currentVelocity, newPosition;
    float smoothTime = 0.5f;

    private void Start()
    {
        newPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position != newPosition)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                newPosition,
                ref currentVelocity,
                smoothTime);
        }
    }

    public void OnUnitMoved(Hex oldHex, Hex newHex)
    {
        // Animate moving unit
        HexMap hexMap = oldHex.HexMap;
        transform.position = hexMap.PositionFromCamera(oldHex);
        newPosition = hexMap.PositionFromCamera(newHex);
        currentVelocity = Vector3.zero;

        transform.SetParent(hexMap.GetGameObjectFromHex(newHex).transform);
        if (Vector3.Distance(transform.position, newPosition) > 2)
        {
            // Big Jump
            transform.position = newPosition;
        }
    }
}
