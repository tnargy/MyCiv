using UnityEngine;

public class MapObjectView : MonoBehaviour
{
    Vector3 currentVelocity, newPosition;
    readonly float smoothTime = 0.5f;

    private void Start()
    {
        newPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, newPosition) > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                newPosition,
                ref currentVelocity,
                smoothTime);
        }
        else
        {
            FindObjectOfType<GameController>().AnimationIsPlaying = false;
        }
    }

    public void OnObjectMoved(Hex oldHex, Hex newHex)
    {
        // Animate moving unit
        HexMap hexMap = oldHex.HexMap;
        transform.position = hexMap.PositionFromCamera(oldHex);
        newPosition = hexMap.PositionFromCamera(newHex);
        if (newHex.isHill)  // Move the Unit up slightly to account for hill
            newPosition.Set(newPosition.x, newPosition.y + 0.15f, newPosition.z);
        currentVelocity = Vector3.zero;

        transform.SetParent(hexMap.GetGameObjectFromHex(newHex).transform);
        if (Vector3.Distance(transform.position, newPosition) > 2)
        {
            // Big Jump
            transform.position = newPosition;
        }
        else
        {
            FindObjectOfType<GameController>().AnimationIsPlaying = true;
        }
    }
}
