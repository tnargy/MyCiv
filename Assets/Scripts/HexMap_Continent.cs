using TMPro;
using UnityEngine;

public class HexMap_Continent: HexMap
{
    [SerializeField]
    int numContinents = 2;
    
    override public void GenerateMap()
    {
        base.GenerateMap();

        int continentSpacing = mapX / numContinents;
        Random.InitState(0); // Seed function for testing

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


        // Add lumpiness Perlin Noise
        float noiseResolution = 0.01f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f,1f), Random.Range(0f,1f));
        float noiseScale = 2f;  // Large number makes more island/lakes

        for (int col = 0; col < mapX; col++)
        {
            for (int row = 0; row < mapY; row++)
            {
                Hex h = GetHexAt(col, row);
                int squareMap = Mathf.Max(mapX, mapY);  // Used to make noise square
                float noise = Mathf.PerlinNoise(
                    ((float)col / squareMap / noiseResolution) + noiseOffset.x, 
                    ((float)row / squareMap / noiseResolution) + noiseOffset.y
                    ) - 0.5f;
                h.Elevation += noise * noiseScale;
            }
        }

        // Simulate rainfall / moisture(prob just Perlin it for now) and set to
        //     plains / grassland + forest
        noiseResolution = 0.05f;
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        noiseScale = 2f;

        for (int col = 0; col < mapX; col++)
        {
            for (int row = 0; row < mapY; row++)
            {
                Hex h = GetHexAt(col, row);
                int squareMap = Mathf.Max(mapX, mapY);  // Used to make noise square
                float noise = Mathf.PerlinNoise(
                    ((float)col / squareMap / noiseResolution) + noiseOffset.x,
                    ((float)row / squareMap / noiseResolution) + noiseOffset.y
                    ) - 0.5f;
                h.Moisture += noise * noiseScale;
            }
        }


        UpdateHexVisuals();
    }

    private void ElevateArea(int q, int r, int range, float centerHeight = 0.8f)
    {
        Hex center = GetHexAt(q,r);
        Hex[] area = GetHexesWithinRangeOf(center, range);
        foreach(Hex h in area)
        {
            //if (h.Elevation < 0)
            //    h.Elevation = 0;
            
            h.Elevation = centerHeight * Mathf.Lerp( 1f, 0.25f, Mathf.Pow(Hex.Distance(center, h) / range,2) );
        }
    }
}
