using QPath;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject, IQPathUnit
{
    public float Movement, MovementRemaining;
    public UNITTYPE UnitType;
    private Queue<Hex> hexPath;

    public bool CanBuildCities = false;

    // TODO This should be moved to central config file
    const bool MOVEMENT_RULES_LIKE_CIV6 = false;

    public Unit(string name, int hp, int strength, float movement, UNITTYPE unitType)
    {
        Name = name;
        HP = hp;
        Strength = strength;
        Movement = movement;
        MovementRemaining = movement;
        UnitType = unitType;
        hexPath = new Queue<Hex>();
    }

    override public void SetHex(Hex newHex)
    {
        if (Hex != null)
        {
            Hex.RemoveUnit(this);
        }
        base.SetHex(newHex);
        Hex.AddUnit(this);
    }

    public Hex[] GetHexPath()
    {
        if (hexPath == null)
            return null;
        Hex[] path = new Hex[hexPath.Count + 1];
        path[0] = Hex;
        hexPath.ToArray().CopyTo(path, 1);
        return path;
    }
    public void SetHexPath(Hex[] path)
    {
        hexPath = new Queue<Hex>(path);
        if (hexPath.Count > 0)
            hexPath.Dequeue();  // Skip current tile.
    }

    internal void RefreshMovement()
    {
        MovementRemaining = Movement;
    }

    public bool UnitWaitingForOrders()
    {
        if ((hexPath == null || hexPath.Count == 0) && MovementRemaining > 0)
        {
            // TODO: Fortify/Alert/SkipTurn
            return true;
        }
        return false;
    }

    /// <summary>
    /// Processes one tile worth of movement for the unit
    /// </summary>
    /// <returns>Returns true if this should be called immediately again.</returns>
    public bool DoMove()
    {
        if (hexPath == null || hexPath.Count == 0 || MovementRemaining <= 0)
        {
            return false;
        }

        float costToEnter = MovementCostToEnterHex(hexPath.Peek());
        if (costToEnter > MovementRemaining && MovementRemaining > Movement && MOVEMENT_RULES_LIKE_CIV6)
        {
            // Can't enter this turn.
            return false;
        }

        Hex newHex = hexPath.Dequeue();
        SetHex(newHex);

        MovementRemaining = Mathf.Max(MovementRemaining - costToEnter, 0);
        return hexPath != null && MovementRemaining > 0;
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

    public void BuildCity()
    {
        if (UnitWaitingForOrders())
        {
            Hex.AddCity();
            // Use up all your movement
            MovementRemaining = 0;
        }
    }
}