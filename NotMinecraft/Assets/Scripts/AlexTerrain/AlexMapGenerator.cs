using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexMapGenerator : MonoBehaviour
{
    public enum DrawingMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawingMode drawingMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;

    // Creates a slider in the editor for persistence
    [Range(0, 1)]
    public float persistence;

    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void generateMap()
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    // When true, means region of the currentHeight of the noise map is found 
                    if (currentHeight <= regions[i].height)
                    {
                        // Sets the color map at index to the corresponding region color
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawingMode == DrawingMode.NoiseMap)
        {
            display.drawTexture(TextureGenerator.heightMapTexture(noiseMap));
        }
        else if (drawingMode == DrawingMode.ColorMap)
        {
            display.drawTexture(TextureGenerator.colorMapTexture(colorMap, mapWidth, mapHeight));
        }
        else if (drawingMode == DrawingMode.Mesh)
        {
            display.drawMesh(MeshGenerator.generateTerrainMesh(noiseMap), TextureGenerator.colorMapTexture(colorMap, mapWidth, mapHeight));
        }
    }

    // Clamping values when they are changed in the editor if they go above or below a certain value
    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

// So this will show up in the inspector
[System.Serializable]
public struct TerrainType
{
    public string terrainName;

    // Height is the range of y values that a specic terrain will occupy,
    // meaning if waters height is 0.4 and ground is 1 that means from 0 to 0.4 will be the water region
    // and 0.4 to 1 will be the land region
    public float height;
    public Color color;
    
}
