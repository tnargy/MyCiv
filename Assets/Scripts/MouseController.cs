﻿using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    HexMap hexMap;
    Vector3 lastMouseGroundPlanePosition;
    Vector3 lastMousePosition;
    Hex[] hexPath;
    public Unit SelectedUnit;
    public City SelectedCity;
    public LayerMask layerMask;
    public GameObject UnitSelectedPanel;
    public GameObject CitySelectedPanel;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;
    LineRenderer lineRenderer;

    private GameController GM;
    private int objIndex = -1;

    private void Start()
    {
        GM = FindObjectOfType<GameController>();
        UnitSelectedPanel.SetActive(false);
        CitySelectedPanel.SetActive(false);

        hexMap = FindObjectOfType<HexMap>();
        lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        Update_CurrentFunc = Update_DetectModeStart;
    }

    private void Update()
    {
        if (GameObject.Find("Middle").GetComponent<Canvas>().enabled)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            ClearUI();
        if (hexPath != null)
            DrawPath(hexPath);
        Update_CurrentFunc();
        Update_Zoom();
        lastMousePosition = Input.mousePosition;
    }

    void ResetUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;
    }

    void ClearUI()
    {
        Update_CurrentFunc = Update_DetectModeStart;
        ClearPath();
        UnitSelectedPanel.SetActive(false);
        CitySelectedPanel.SetActive(false);
        SelectedUnit = null;
        SelectedCity = null;
    }

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SelectMapObject(MouseToHex());
        }
        else if (SelectedUnit != null && Input.GetMouseButton(1))
        {
            Update_CurrentFunc = Update_UnitMovement;
        }
        else if (SelectedCity == null && Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, lastMousePosition) > 2f)
        {
            Update_CurrentFunc = Update_CameraDrag;
            lastMouseGroundPlanePosition = GetHitPos();
            Update_CurrentFunc();
        }
    }

    private void SelectMapObject(Hex hex)
    {
        if (hex == null)
            return;

        Unit[] units = hex.Units;
        City city = hex.City;

        if (units != null && units.Length > 0)
        {
            objIndex++;
            if (objIndex >= units.Length)
            {
                if (city != null)
                    SelectCity(city);
                objIndex = -1;
            }
            else
                SelectUnit(units, objIndex);
        }
        else if (city != null)
        {
            SelectCity(city);
        }
        else
        {
            ClearUI();
            objIndex = -1;
        }
    }

    void SelectUnit(Unit[] unit, int objIndex)
    {
        SelectedCity = null;
        SelectedUnit = unit[objIndex];
        UnitSelectedPanel.SetActive(true);
        CitySelectedPanel.SetActive(false);
        hexPath = SelectedUnit.GetHexPath() ?? null;
    }

    private void SelectCity(City city)
    {
        ClearUI();
        SelectedCity = city;
        SelectedUnit = null;
        CitySelectedPanel.SetActive(true);
        UnitSelectedPanel.SetActive(false);
    }

    void Update_UnitMovement()
    {
        if (Input.GetMouseButtonUp(1) || SelectedUnit == null)
        {
            if (SelectedUnit != null)
            {
                SelectedUnit.SetHexPath(hexPath);
                StartCoroutine(GM.DoUnitMoves(SelectedUnit));
            }
            ResetUpdateFunc();
            return;
        }
        else
        {
            hexPath = QPath.QPath.FindPath<Hex>(hexMap, SelectedUnit, SelectedUnit.Hex, MouseToHex(), Hex.CostEstimate);
        }
    }

    private void ClearPath()
    {
        hexPath = null;
        lineRenderer.enabled = false;
    }

    void Update_CameraDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ResetUpdateFunc();
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

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask))
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                return null;
            }
            GameObject hexObj = hitInfo.rigidbody.gameObject;
            Hex hex = hexMap.GetHexFromGameObject(hexObj);

            return hex;
        }

        return null;
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
