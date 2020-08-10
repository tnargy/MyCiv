using QPath;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexMap : MonoBehaviour, IQPathWorld
{
    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;

    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrasslands;
    public Material MatMountain;
    public Material MatDesert;

    public GameObject UnitWarriorPrefab;

    public static float mapHeightLimit;
    public int MapX = 60, MapY = 30;
    public float HeightMountain = 0.85f, HeightHill = 0.6f, HeightFlat = 0f;
    public float MoistureJungle = 0.66f, MoistureForest = 0.33f;
    public float MoistureGrasslands = 0f, MoisturePlains = -0.5f;
    private Hex[,] hexes;
    private HashSet<Unit> units;
    public Dictionary<Hex, GameObject> HexToGameObjectMap;
    private Dictionary<Unit, GameObject> unitToGameObjectMap;


    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (units != null)
            {
                foreach (Unit unit in units)
                {
                    unit.DoTurn();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (units != null)
            {
                foreach (Unit unit in units)
                {
                    unit.DUMMY_PATHING_FUNCTION();
                }
            }
        }
    }

    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array not yet instanciated!");
            return null;
        }

        x %= MapX;
        if (x < 0)
            x += MapX;

        Hex hex;
        try
        {
            hex = hexes[x, y];
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.LogWarning("Edge of screen");
            return null;
        }

        return hex;
    }

    virtual public void GenerateMap()
    {
        hexes = new Hex[MapX, MapY];
        HexToGameObjectMap = new Dictionary<Hex, GameObject>();

        // Generate Ocean Map
        for (int col = 0; col < MapX; col++)
        {
            for (int row = 0; row < MapY; row++)
            {
                Hex h = new Hex(this, col, row)
                {
                    Elevation = -0.5f
                };
                Vector3 pos = PositionFromCamera(h);

                GameObject hexObj = Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    transform);
                hexObj.name = $"Hex: {col}, {row}";
                hexObj.GetComponent<HexBehavior>().hex = h;
                hexObj.GetComponent<HexBehavior>().hexMap = this;

                if (hexObj.GetComponentInChildren<TextMeshPro>() != null)
                    hexObj.GetComponentInChildren<TextMeshPro>().text = $"{col}, {row}";

                hexes[col, row] = h;
                HexToGameObjectMap[h] = hexObj;

            }
        }

        mapHeightLimit = hexes[0, 0].Vert_spacing * MapY;
        UpdateHexVisuals();
    }

    public Vector3 PositionFromCamera(Hex h)
    {
        float mapWidth = h.Horz_spacing * MapX;

        Vector3 position = h.Position();

        // Goal between -0.5 to 0.5
        float howManyWidthsFromCamera =
            (position.x - Camera.main.transform.position.x) / mapWidth;

        int widthToFix = Mathf.RoundToInt(howManyWidthsFromCamera);

        position.x -= widthToFix * mapWidth;
        return position;
    }

    public void UpdateHexVisuals()
    {
        for (int col = 0; col < MapX; col++)
        {
            for (int row = 0; row < MapY; row++)
            {
                Hex h = hexes[col, row];
                GameObject hexObj = HexToGameObjectMap[h];

                MeshRenderer mr = hexObj.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexObj.GetComponentInChildren<MeshFilter>();

                // Moiture
                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain)
                {
                    if (h.Moisture >= MoistureJungle)
                    {
                        mr.material = MatGrasslands;
                        Vector3 p = hexObj.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }
                        h.MovementCost = 2;
                        Instantiate(ForestPrefab, p, Quaternion.identity, hexObj.transform);
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrasslands;
                        Vector3 p = hexObj.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }
                        h.MovementCost = 2;
                        Instantiate(ForestPrefab, p, Quaternion.identity, hexObj.transform);
                    }
                    else if (h.Moisture >= MoistureGrasslands)
                    {
                        mr.material = MatGrasslands;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                    }
                    else
                    {
                        mr.material = MatDesert;
                    }
                }

                // Elevation
                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                    h.MovementCost = -1;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                    h.MovementCost = 2;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshFlat;
                    h.MovementCost = 1;
                }
                else
                {
                    mr.material = MatOcean;
                    mf.mesh = MeshWater;
                    h.MovementCost = -1;
                }
            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex center, int range)
    {
        List<Hex> results = new List<Hex>();
        for (int dx = -range; dx < range - 1; dx++)
        {
            for (int dy = Mathf.Max(-range + 1, -dx - range); dy < Mathf.Min(range, -dx + range - 1); dy++)
            {
                results.Add(GetHexAt(center.Q + dx, center.R + dy));
            }
        }
        return results.ToArray();
    }

    public void SpawnUnitAt(Unit unit, GameObject prefab, int q, int r)
    {
        if (units == null)
        {
            units = new HashSet<Unit>();
            unitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }

        Hex h = GetHexAt(q, r);
        Transform hexTransform = HexToGameObjectMap[h].transform;
        GameObject unitObj = Instantiate(
            prefab,
            hexTransform.position,
            Quaternion.identity,
            hexTransform);

        units.Add(unit);
        unitToGameObjectMap.Add(unit, unitObj);
        unit.SetHex(h);
        unit.OnUnitMoved += unitObj.GetComponent<UnitView>().OnUnitMoved;
    }
}
