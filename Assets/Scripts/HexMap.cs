using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrasslands;
    public Material MatMountain;
    public Material MatDesert;

    public static float mapHeightLimit;
    public int mapX = 60, mapY = 30;
    public float HeightMountain = 0.85f, HeightHill = 0.6f, HeightFlat = 0.0f;
    public float MoistureJungle = 1f, MoistureForest = 0.8f, MoistureGrasslands = 0.33f, MoisturePlains = 0.0f;
    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;


    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array not yet instanciated!");
            return null;
        }

        x %= mapX;
        if (x < 0)
            x += mapX;

        Hex hex = hexes[x, y];
        return hex ?? null;
    }

    virtual public void GenerateMap()
    {
        hexes = new Hex[mapX, mapY];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();

        // Generate Ocean Map
        for (int col = 0; col < mapX; col++)
        {
            for (int row = 0; row < mapY; row++)
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
                hexToGameObjectMap[h] = hexObj;

            }
        }

        mapHeightLimit = hexes[0, 0].Vert_spacing * mapY;
        UpdateHexVisuals();
    }

    public Vector3 PositionFromCamera(Hex h)
    {
        float mapWidth = h.Horz_spacing * mapX;

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
        for (int col = 0; col < mapX; col++)
        {
            for (int row = 0; row < mapY; row++)
            {
                Hex h = hexes[col, row];
                GameObject hexObj = hexToGameObjectMap[h];

                MeshRenderer mr = hexObj.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexObj.GetComponentInChildren<MeshFilter>();

                // Elevation
                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mr.material = MatPlains;
                    mf.mesh = MeshHill;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    mr.material = MatPlains;
                    mf.mesh = MeshFlat;
                }
                else
                {
                    mr.material = MatOcean;
                    mf.mesh = MeshWater;
                }

                // Moiture
                if (h.Elevation >= HeightFlat)
                {
                    if (h.Moisture >= MoistureJungle)
                    {
                        mr.material = MatGrasslands;
                        // TODO: Spawn Jungle
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrasslands;
                        // TODO: Spawn Forests
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
}
