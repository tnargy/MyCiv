using System;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    bool isDraggingCamera = false;
    Vector3 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
        Vector3 hitPos = GetHitPos();
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0))
        {
            isDraggingCamera = true;
            lastMousePosition = hitPos;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isDraggingCamera = false;
        }

        if (isDraggingCamera)
        {
            Vector3 diff = lastMousePosition - hitPos;
            Camera.main.transform.Translate(diff, Space.World);

            lastMousePosition = GetHitPos();
        }
        else if (Mathf.Abs(scrollAmount) > 0.01f)
        {
            float minHeight = 2, maxHeight = 20;

            Vector3 p = Camera.main.transform.position;
            Vector3 dir = hitPos - p;

            if ((scrollAmount > 0 && 2 < p.y) || (scrollAmount < 0 && p.y < 20))
                Camera.main.transform.Translate(dir * scrollAmount, Space.World);

            p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(
                p.x,
                Mathf.Clamp(p.y, minHeight, maxHeight),
                p.z);

            Camera.main.transform.rotation = Quaternion.Euler(
                Mathf.Lerp(20, 90, p.y / (maxHeight / 1.5f)),
                Camera.main.transform.rotation.eulerAngles.y,
                Camera.main.transform.rotation.eulerAngles.z);
        }
    }

    private Vector3 GetHitPos()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mouseRay.direction.y >= 0)
        {
            Debug.LogError("Mouse pointing up???");
            isDraggingCamera = false;
        }
        float rayLen = mouseRay.origin.y / mouseRay.direction.y;
        return mouseRay.origin - (mouseRay.direction * rayLen);
    }
}
