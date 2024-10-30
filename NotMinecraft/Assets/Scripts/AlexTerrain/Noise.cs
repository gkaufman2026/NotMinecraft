using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    // Returns a 2D Array of floats 
    public static float[,] generateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                float mapX = i / scale;
                float mapY = j / scale;

                float perlinValue = Mathf.PerlinNoise(mapX, mapY);
                noiseMap[j, i] = perlinValue;
            }
        }

        return noiseMap;
    }
}
