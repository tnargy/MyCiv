using QPath;
using System;
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

    // TODO Need prop to track type (plain, grassland ...)
    // TODO Need prop to track detail (forest, mine, farm ...)

    public readonly HexMap HexMap;
    private readonly float diameter, width;
    static readonly float width_multiplier = Mathf.Sqrt(3) / 2;

    HashSet<Unit> units;
    Hex[] neighbours;

    public Vector3 Position()
    {
        return new Vector3(
            Horz_spacing * (Q + R / 2f),
            0,
            Vert_spacing * R);
    }

    public static float CostECostEstimate(IQPathTile a, IQPathTile b)
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
                HexMap.GetHexAt(Q - 1, R + 1)};
            neighbours = (Hex[])neighbours.Where(x => x != null);
            Debug.Log("Test");
        }
        return neighbours;
    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        throw new System.NotImplementedException();
    }
}
