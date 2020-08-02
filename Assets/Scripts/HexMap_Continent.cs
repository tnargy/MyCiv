using TMPro;
using UnityEngine;

public class HexMap_Continent: HexMap
{
    override public void GenerateMap()
    {
        base.GenerateMap();

        //Make some kind of raised area
        ElevateArea(21, 15, 4);

        //Add lumpiness Perlin Noise?

        //Set mesh to mountain/ flat / water based on height

        //Simulate rainfall / moisture(prob just Perlin it for now) and set to
        //    plains / grasland + forest

        UpdateHexVisuals();
    }

    private void ElevateArea(int q, int r, int radius)
    {
        Hex center = GetHexAt(q,r);
        Hex[] area = GetHexesWithinRadiusOf(center, radius);
        foreach(Hex h in area)
        {
            h.Elevation = 0.5f;
        }
    }
}
