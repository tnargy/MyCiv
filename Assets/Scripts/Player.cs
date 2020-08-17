using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string Name;
    private HashSet<Unit> units;
    private HashSet<City> cities;
    private Dictionary<Unit, GameObject> unitToGameObjectMap;
    private Dictionary<City, GameObject> cityToGameObjectMap;

    public Player(string name)
    {
        Name = name;
        units = new HashSet<Unit>();
        unitToGameObjectMap = new Dictionary<Unit, GameObject>();

        cities = new HashSet<City>();
        cityToGameObjectMap = new Dictionary<City, GameObject>();
    }

    public Unit[] Units { get => units.ToArray(); }
    public City[] Cities { get => cities.ToArray(); }
    public Dictionary<Unit, GameObject> UnitToGameObjectMap { get => unitToGameObjectMap; }

    public void AddUnit(Unit unit, GameObject unitObj)
    {
        units.Add(unit);
        unitToGameObjectMap.Add(unit, unitObj);
    }

    public void AddCity(City city, GameObject cityObj)
    {
        cities.Add(city);
        cityToGameObjectMap.Add(city, cityObj);
    }
    
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unitToGameObjectMap.Remove(unit);
    }

    public void RemoveCity(City city)
    {
        cities.Remove(city);
        cityToGameObjectMap.Remove(city);
    }
}