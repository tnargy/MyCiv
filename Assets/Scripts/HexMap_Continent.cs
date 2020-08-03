using TMPro;
using UnityEngine;

public class HexMap_Continent: HexMap
{
    override public void GenerateMap()
    {
        base.GenerateMap();

        int numContinents = 2;
        int continentSpacing = 20;
        for (int c = 0; c < numContinents; c++)
        {
            // Make some kind of raised area
            int numSplats = Random.Range(4, 8);
            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 8);
                int y = Random.Range(range, mapY - range);
                int x = Random.Range(0, 10) - y / 2 + (c * continentSpacing);
                ElevateArea(x, y, range);
            }
        }


        // Add lumpiness Perlin Noise?

           
        // Set mesh to mountain/ flat / water based on height
           
        // Simulate rainfall / moisture(prob just Perlin it for now) and set to
        //     plains / grasland + forest

        UpdateHexVisuals();
    }

    private void ElevateArea(int q, int r, int range, float centerHeight = 1f)
    {
        Hex center = GetHexAt(q,r);
        Hex[] area = GetHexesWithinRangeOf(center, range);
        foreach(Hex h in area)
        {
            //if (h.Elevation < 0)
            //    h.Elevation = 0;
            
            h.Elevation += centerHeight * Mathf.Lerp( 1f, 0.25f, Hex.Distance(center, h) / range );
        }
    }
}
