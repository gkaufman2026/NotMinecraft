using UnityEngine;
using System.Collections;

public static class Noise
{
    // Returns a 2D Array of floats 
    public static float[,] generateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-10000, 10000) + offset.x;
            float offsetY = rand.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Higher the frequency, the further apart the sample points will be,
                // meaning the height values will change more rapidly
                for (int i = 0; i < octaves; i++)
                {
                    float mapX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float mapY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(mapX, mapY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    // Decreases each octave, since persisten is < 1
                    amplitude *= persistance;

                    // Increases each octave, since lacunarity is > 1
                    frequency *= lacunarity;
                }

                // Sets new max values if the passed in value is greater than the current max or minimum
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
