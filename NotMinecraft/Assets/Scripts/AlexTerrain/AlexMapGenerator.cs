using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexMapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public bool autoUpdate;

    public void generateMap()
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapWidth, mapHeight, noiseScale);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.drawNoiseMap(noiseMap);
    }
}
