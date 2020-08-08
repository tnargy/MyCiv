using UnityEngine;

public class UnitView : MonoBehaviour
{
    Vector3 currentVelocity, newPosition;
    float smoothTime = 0.5f;

    private void Start()
    {
        newPosition = this.transform.position;
    }

    private void Update()
    {
        if (this.transform.position != newPosition)
        {
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position,
                newPosition,
                ref currentVelocity,
                smoothTime);
        }
    }

    public void OnUnitMoved(Hex oldHex, Hex newHex)
    {
        // Animate moving unit
        HexMap hexMap = oldHex.HexMap;
        this.transform.position = hexMap.PositionFromCamera(oldHex);
        newPosition = hexMap.PositionFromCamera(newHex);
        currentVelocity = Vector3.zero;

        this.transform.SetParent(hexMap.hexToGameObjectMap[newHex].transform);
        if (Vector3.Distance(this.transform.position, newPosition) > 2)
        {
            // Big Jump
            this.transform.position = newPosition;
        }
    }
}
