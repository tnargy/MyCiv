using UnityEngine;

public class MouseController : MonoBehaviour
{
    Vector3 lastMouseGroundPlanePosition;
    Vector3 lastMousePosition;
    Unit selectedUnit = null;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    private void Start()
    {
        Update_CurrentFunc = Update_DetectModeStart;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelUpdateFunc();

        Update_CurrentFunc();
        Update_Zoom();
        lastMousePosition = Input.mousePosition;
    }

    void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;

        // Cleanup UI stuff
    }

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // TODO: select unit
        }
        else if (selectedUnit != null && Input.GetMouseButton(1))
        {
            Update_CurrentFunc = Update_UnitMovement;
        }
        else if (Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, lastMousePosition) > 2f)
        {
            Update_CurrentFunc = Update_CameraDrag;
            lastMouseGroundPlanePosition = GetHitPos();
            Update_CurrentFunc();
        }
    }

    void Update_UnitMovement()
    {
        if (Input.GetMouseButtonUp(1))
        {
            // TODO: copy pathfinding path to unit's movement queue
            CancelUpdateFunc();
            return;
        }
    }

    // Update is called once per frame
    void Update_CameraDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CancelUpdateFunc();
            return;
        }

        Vector3 diff = lastMouseGroundPlanePosition - GetHitPos();
        Camera.main.transform.Translate(diff, Space.World);

        lastMouseGroundPlanePosition = GetHitPos();
    }
    void Update_Zoom()
    {
        float minHeight = 2, maxHeight = 20;
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

        Vector3 p = Camera.main.transform.position;
        Vector3 dir = GetHitPos() - p;

        if ((scrollAmount > 0 && 2 < p.y) || (scrollAmount < 0 && p.y < 20))
            Camera.main.transform.Translate(dir * scrollAmount, Space.World);

        p = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(
            p.x,
            Mathf.Clamp(p.y, minHeight, maxHeight),
            p.z);

        //Change angle if Ctrl held down
        p = Camera.main.transform.position;
        Camera.main.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(20, 90, p.y / (maxHeight / 1.5f)),
            Camera.main.transform.rotation.eulerAngles.y,
            Camera.main.transform.rotation.eulerAngles.z);
    }

    private Vector3 GetHitPos()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLen = mouseRay.origin.y / mouseRay.direction.y;
        return mouseRay.origin - (mouseRay.direction * rayLen);
    }
}
