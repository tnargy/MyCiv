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

    public static float mapHeightLimit;
    public int mapX = 60, mapY = 30;
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

        Hex hex = hexes[x % mapY, y];
        return hex ?? null;
    }

    virtual public void GenerateMap()
    {
        hexes = new Hex[mapX, mapY];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();

        //Generate Ocean Map
        for (int col = 0; col < mapX; col++)
        {
            for (int row = 0; row < mapY; row++)
            {
                Hex h = new Hex(col, row);
                h.Elevation = -1f;
                Vector3 pos = PositionFromCamera(h);

                GameObject hexObj = Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    transform);
                hexObj.name = string.Format("Hex: {0}, {1}", col, row);
                hexObj.GetComponent<HexBehavior>().hex = h;
                hexObj.GetComponent<HexBehavior>().hexMap = this;

                hexObj.GetComponentInChildren<TextMeshPro>().text =
                    string.Format("{0}, {1}", col, row);

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

        //Goal between -0.5 to 0.5
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
                if (h.Elevation >= 0)
                {
                    mr.material = MatGrasslands;
                }
                else
                {
                    mr.material = MatOcean;
                }
                
                MeshFilter mf = hexObj.GetComponentInChildren<MeshFilter>();
                mf.mesh = MeshWater;
            }
        }
    }

    public Hex[] GetHexesWithinRadiusOf(Hex center, int radius)
    {
        List<Hex> results = new List<Hex>();
        for (int dx = -radius; dx < radius-1; dx++)
        {
            for (int dy = Mathf.Max(-radius+1, -dx-radius); dy < Mathf.Min(radius, -dx+radius-1); dy++)
            {
                results.Add(hexes[center.q + dx, center.r + dy]);
            }
        }
        return results.ToArray();
    }
}
