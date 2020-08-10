﻿using QPath;
using System.Collections.Generic;
using UnityEngine;

public class Unit : IQPathUnit
{
    public string Name = "Unnamed";
    public int HP = 100, Strength = 8;
    public int Movement = 2;
    public int MovementRemaining = 2;

    public Hex Hex { get; protected set; }

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    Queue<Hex> hexPath;

    // TODO This should be moved to central config file
    const bool MOVEMENT_RULES_LIKE_CIV6 = false;
    
    public void DUMMY_PATHING_FUNCTION()
    {
        QPath.QPath.FindPath(Hex.HexMap, this, Hex, Hex.HexMap.GetHexAt(Hex.Q + 5, Hex.R), Hex.CostECostEstimate);
    }

    public void SetHexPath(Hex[] hexPath)
    {
        this.hexPath = new Queue<Hex>(hexPath);
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

    public int MovementCostToEnterHex(Hex hex)
    {
        // TODO Override base movement cost based on
        // our movement mode + tile type
        return hex.BaseMovementCost();
    }

    public float AggregateTurnsToEnterHex(Hex hex, float turnsToDate)
    {
        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement;

        if (baseTurnsToEnterHex > 1)
            baseTurnsToEnterHex = 1;

        float turnsRemaining = MovementRemaining / Movement;

        float turnsToDateWhole = Mathf.Floor(turnsToDate);
        float turnsToDateFraction = turnsToDate - turnsToDateWhole;

        if (turnsToDateFraction < 0.01f || turnsToDateFraction > 0.99f)
        {
            Debug.LogWarning("Floating point drift.");
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
