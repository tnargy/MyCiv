using QPath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hex : IQPathTile
{
    public Hex(HexMap hexMap, int q, int r, float radius = 1f)
    {
        // q + r + s = 0
        // s = -(q + r)

        this.Q = q;
        this.R = r;
        this.S = -(q + r);
        this.diameter = radius * 2;
        this.width = width_multiplier * diameter;
        this.Horz_spacing = width;
        this.Vert_spacing = diameter * 0.75f;
        this.HexMap = hexMap;
    }


    public readonly int Q, R, S;    // Coordinates
    public float Elevation { get; set; }
    public float Moisture { get; set; }
    public float Horz_spacing { get; }
    public float Vert_spacing { get; }
    public City City { get; protected set; }
    private HashSet<Unit> units;
    private Hex[] neighbours;

    public enum TERRAINTYPE { Water, Dessert, Plains, Grassland, Forest, Jungle, Mountain };
    public TERRAINTYPE Terrain = TERRAINTYPE.Water;
    public bool isHill = false;

    public readonly HexMap HexMap;
    private readonly float diameter, width;
    static readonly float width_multiplier = Mathf.Sqrt(3) / 2;

    public Unit[] Units
    {
        get
        {
            if (units == null)
                return null;
            return units.ToArray();
        }
    }

    public override string ToString()
    {
        return $"Hex: ({Q}, {R})";
    }

    public Vector3 Position()
    {
        return new Vector3(
            Horz_spacing * (Q + R / 2f),
            0,
            Vert_spacing * R);
    }

    public static float CostEstimate(IQPathTile a, IQPathTile b)
    {
        return Distance((Hex)a, (Hex)b);
    }

    public static float Distance(Hex a, Hex b)
    {
        int dQ = Mathf.Abs(a.Q - b.Q);
        if (dQ > a.HexMap.MapX / 2)
            dQ = a.HexMap.MapX - dQ;

        return
            Mathf.Max(
                dQ,
                Mathf.Abs(a.R - b.R),
                Mathf.Abs(a.S - b.S));
    }

    public void AddUnit(Unit unit)
    {
        if (units == null)
            units = new HashSet<Unit>();
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (units != null)
            units.Remove(unit);
    }

    public void AddCity(string input)
    {
        City = new City(this, input);
        HexMap.GM.AddCity(City);
    }

    public float BaseMovementCost()
    {
        float baseMovement;
        switch (Terrain)
        {
            case TERRAINTYPE.Water:
            case TERRAINTYPE.Mountain:
                baseMovement = -1;
                break;
            case TERRAINTYPE.Forest:
            case TERRAINTYPE.Jungle:
                baseMovement = 2;
                break;
            case TERRAINTYPE.Dessert:
            case TERRAINTYPE.Plains:
            case TERRAINTYPE.Grassland:
            default:
                baseMovement = 1;
                break;
        }
        return baseMovement;
    }

    public IQPathTile[] GetNeighbours()
    {
        if (neighbours == null)
        {
            neighbours = new Hex[] {
                HexMap.GetHexAt(Q + 1, R + 0),
                HexMap.GetHexAt(Q - 1, R + 0),
                HexMap.GetHexAt(Q + 0, R + 1),
                HexMap.GetHexAt(Q + 0, R - 1),
                HexMap.GetHexAt(Q + 1, R - 1),
                HexMap.GetHexAt(Q - 1, R + 1)}
                .Where(x => x != null).ToArray<Hex>();
        }
        return neighbours;
    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        return ((Unit)theUnit).AggregateTurnsToEnterHex(this, costSoFar);
    }
}
