using QPath;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : IQPathUnit
{
    public string Name = "Unnamed";
    public int HP = 100, Strength = 8;
    public float Movement = 2f;
    public int MovementRemaining = 2;

    public Hex Hex { get; protected set; }

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    Queue<Hex> hexPath;

    // TODO This should be moved to central config file
    const bool MOVEMENT_RULES_LIKE_CIV6 = false;

    public void DUMMY_PATHING_FUNCTION()
    {
        Hex[] pathTiles = QPath.QPath.FindPath<Hex>(Hex.HexMap,
                                                      this,
                                                      Hex,
                                                      Hex.HexMap.GetHexAt(Hex.Q + 5, Hex.R),
                                                      Hex.CostEstimate);
        Debug.Log($"Got pathfinding path length of {pathTiles.Length}");
        SetHexPath(pathTiles);
    }

    public void ClearHexPath()
    {
        hexPath = new Queue<Hex>();
    }

    public void SetHexPath(Hex[] path)
    {
        hexPath = new Queue<Hex>(path);
        if (hexPath.Count > 0)
            hexPath.Dequeue();  // Skip current tile.
    }

    public void SetHex(Hex newHex)
    {
        Hex oldHex = Hex;
        if (Hex != null)
            Hex.RemoveUnit(this);
        Hex = newHex;
        Hex.AddUnit(this);

        OnUnitMoved?.Invoke(oldHex, newHex);
    }

    public void DoTurn()
    {
        if (hexPath == null || hexPath.Count == 0)
        {
            return;
        }

        Hex newHex = hexPath.Dequeue();

        SetHex(newHex);
    }

    public float MovementCostToEnterHex(Hex hex)
    {
        return hex.BaseMovementCost();
    }

    public float AggregateTurnsToEnterHex(Hex hex, float turnsToDate)
    {
        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement;

        if (baseTurnsToEnterHex > 1)
            baseTurnsToEnterHex = 1;
        else if (baseTurnsToEnterHex < 0)
            return -1;  // Impassible

        float turnsRemaining = MovementRemaining / Movement;

        float turnsToDateWhole = Mathf.Floor(turnsToDate);
        float turnsToDateFraction = turnsToDate - turnsToDateWhole;

        if ((turnsToDateFraction > 0f && turnsToDateFraction < 0.01f) || turnsToDateFraction > 0.99f)
        {
            Debug.LogWarning($"Floating point drift: {turnsToDateFraction}");
            turnsToDateWhole += Mathf.Round(turnsToDateFraction);
            turnsToDateFraction = 0f;
        }

        float turnsUsedAfterThisMove = turnsToDateFraction + baseTurnsToEnterHex;
        if (turnsUsedAfterThisMove > 1)
        {
            // Not enough movement to complete action
            if (MOVEMENT_RULES_LIKE_CIV6)
            {
                // Not allowed to enter hex this move
#pragma warning disable CS0162 // Unreachable code detected
                if (turnsToDateFraction != 0)
#pragma warning restore CS0162 // Unreachable code detected
                {
                    // Idle for rest of turn
                    turnsToDateWhole += 1;
                    turnsToDateFraction = 0;
                }
                turnsUsedAfterThisMove = baseTurnsToEnterHex;
            }
            else
            {
                turnsUsedAfterThisMove = 1;
            }
        }

        return turnsToDateWhole + turnsUsedAfterThisMove;
    }

    /// <summary>
    /// Turn cost to enter a hex (ie. 0.5 turns if a movement cost 1 and we have 2 max
    /// </summary>
    public float CostToEnterHex(IQPathTile sourceTile, IQPathTile destinationTile)
    {
        return 1f;
    }
}
