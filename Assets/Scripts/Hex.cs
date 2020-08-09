using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QPath;

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

    // TODO Need prop to track type (plain, grassland ...)
    // TODO Need prop to track detail (forest, mine, farm ...)

    public readonly HexMap HexMap;
    private readonly float diameter, width;
    static readonly float width_multiplier = Mathf.Sqrt(3) / 2;

    HashSet<Unit> units;

    public Vector3 Position()
    {
        return new Vector3(
            Horz_spacing * (Q + R / 2f),
            0,
            Vert_spacing * R);
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
        else
            units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (units != null)
            units.Remove(unit);
    }

    public Unit[] Units()
    {
        return units.ToArray();
    }

    public int BaseMovementCost()
    {
        // TODO Factor in terrain type & features
        return 1;
    }

    public IQPathTile[] GetNeighbors()
    {
        throw new System.NotImplementedException();
    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        throw new System.NotImplementedException();
    }
}
