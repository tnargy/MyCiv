using UnityEngine;

public class HexMap_Continent : HexMap
{
    [SerializeField]
    int numContinents = 2;

    override public void GenerateMap()
    {
        base.GenerateMap();
        
        // Create Continents above sea level
        int continentSpacing = MapX / numContinents;
        for (int c = 0; c < numContinents; c++)
        {
            // Make some kind of raised area
            int numSplats = Random.Range(4, 8);
            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 8);
                int y = Random.Range(range, MapY - range);
                int x = Random.Range(0, 10) - y / 2 + (c * continentSpacing);

                ElevateArea(x, y, range);
            }
        }

        // Add Elevation with Perlin Noise
        float noiseResolution = 0.01f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        float noiseScale = 2f;  // Large number makes more island/lakes

        for (int col = 0; col < MapX; col++)
        {
            for (int row = 0; row < MapY; row++)
            {
                Hex h = GetHexAt(col, row);
                int squareMap = Mathf.Max(MapX, MapY);  // Used to make noise square
                float noise = Mathf.PerlinNoise(
                    ((float)col / squareMap / noiseResolution) + noiseOffset.x,
                    ((float)row / squareMap / noiseResolution) + noiseOffset.y
                    ) - 0.5f;
                h.Elevation += noise * noiseScale;
            }
        }

        // Add Moisture with Perlin Noise
        noiseResolution = 0.05f;
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        noiseScale = 2f;

        for (int col = 0; col < MapX; col++)
        {
            for (int row = 0; row < MapY; row++)
            {
                Hex h = GetHexAt(col, row);
                int squareMap = Mathf.Max(MapX, MapY);  // Used to make noise square
                float noise = Mathf.PerlinNoise(
                    ((float)col / squareMap / noiseResolution) + noiseOffset.x,
                    ((float)row / squareMap / noiseResolution) + noiseOffset.y
                    ) - 0.5f;
                h.Moisture += noise * noiseScale;
            }
        }

        UpdateHexVisuals();
        SpawnPlayer();
    }

    private void SpawnPlayer(bool zoomCamera = true)
    {
        bool respawn = true;
        Hex spawnHex = GetHexAt(0, 0);
        while (respawn)
        {
            respawn = false;
            spawnHex = GetHexAt(Random.Range(2, MapX - 2), Random.Range(2, MapY - 2));
            Hex[] spawnArea = GetHexesWithinRangeOf(spawnHex, 2);
            foreach (Hex h in spawnArea)
            {
                if (h.Elevation < HeightFlat)
                    respawn = true;
            }
            if (spawnHex.Terrain != Hex.TERRAINTYPE.Plains || spawnHex.isHill)
                respawn = true;
        }
        GM.SpawnUnitAt(UNITTYPE.Warrior, spawnHex, GetGameObjectFromHex(spawnHex).transform);
        if (zoomCamera)
            Camera.main.GetComponent<CameraMotion>().MoveToHex(spawnHex);
    }

    private void ElevateArea(int q, int r, int range, float centerHeight = 0.8f)
    {
        Hex center = GetHexAt(q, r);
        Hex[] area = GetHexesWithinRangeOf(center, range);
        foreach (Hex h in area)
        {
            h.Elevation = centerHeight * Mathf.Lerp(
                1f,
                0.25f,
                Mathf.Pow(Hex.Distance(center, h) / range, 2)
                );
        }
    }
}
