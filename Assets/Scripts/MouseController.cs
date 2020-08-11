using UnityEngine;

public class MouseController : MonoBehaviour
{
    HexMap hexMap;
    Vector3 lastMouseGroundPlanePosition;
    Vector3 lastMousePosition;
    Unit selectedUnit = null;
    Hex[] hexPath;
    public LayerMask LMHex;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;
    private Hex hexLastUnderMouse;
    private Hex hexUnderMouse;
    LineRenderer lineRenderer;

    private void Start()
    {
        hexMap = GameObject.FindObjectOfType<HexMap>();
        lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        Update_CurrentFunc = Update_DetectModeStart;
    }

    private void Update()
    {
        hexUnderMouse = MouseToHex();
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelUpdateFunc();
        if (hexPath != null)
            DrawPath(hexPath);
        Update_CurrentFunc();
        Update_Zoom();
        lastMousePosition = Input.mousePosition;
        hexLastUnderMouse = hexUnderMouse;
    }

    void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;
        lineRenderer.enabled = false;
        // TODO Cleanup UI stuff
    }

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnit();
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
        if (Input.GetMouseButtonUp(1)  || selectedUnit == null)
        {
            if (selectedUnit != null)
            {
                selectedUnit.SetHexPath(hexPath);
                hexPath = null;
            }

            CancelUpdateFunc();
            return;
        }

        if (hexPath == null || hexUnderMouse != hexLastUnderMouse)
        {
            hexPath = QPath.QPath.FindPath<Hex>(hexMap, selectedUnit, selectedUnit.Hex, hexUnderMouse, Hex.CostEstimate);
        }
    }
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

    private Hex MouseToHex()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, LMHex.value))
        {
            GameObject hexObj = hitInfo.rigidbody.gameObject;
            Hex hex = hexMap.GetHexFromGameObject(hexObj);
            
            return hex;
        }

        return null;
    }

    void SelectUnit()
    {
        Unit[] units = hexUnderMouse.Units;
        if (units != null && units.Length > 0)
        {
            selectedUnit = units[0];
            Debug.Log("Selected Unit");
        }
        hexPath = selectedUnit.GetHexPath() ?? null;
    }

    void DrawPath(Hex[] hexPath)
    {
        if (hexPath.Length == 0)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] lineTiles = new Vector3[hexPath.Length];
        lineRenderer.positionCount = lineTiles.Length;
        for (int i = 0; i < hexPath.Length; i++)
        {
            GameObject hexObj = hexMap.GetGameObjectFromHex(hexPath[i]);
            lineTiles[i] = hexObj.transform.position + (Vector3.up * 0.1f);
        }
        lineRenderer.SetPositions(lineTiles);
    }

}
