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

    // 241 is choosen here because of Unity's mesh vertice limit of 65025, 240 is divisable by 2,4,6,8,10,and 12
    // making it optimal for chunk threading, 241 is choosen because our actual size will be the size - 1
    // When looping through the map, we are incrementing by i, creating a vertex at each point
    // The created mesh can be simplified by making i > 1, doing this will decrease the totla number of verticies
    // however i must be a factor of the map's widthm if i is not a factor, we will leave the scope of the map array
    // since we are looping through the width and height of the map
    // width and height must both be less than 255 since 255^2 = 65025
    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int meshSimplification;

    public float noiseScale;

    public float meshHeightScalar;
    public AnimationCurve meshHeightCurve;

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
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    // When true, means region of the currentHeight of the noise map is found 
                    if (currentHeight <= regions[i].height)
                    {
                        // Sets the color map at index to the corresponding region color
                        colorMap[y * mapChunkSize + x] = regions[i].color;
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
            display.drawTexture(TextureGenerator.colorMapTexture(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawingMode == DrawingMode.Mesh)
        {
            display.drawMesh(MeshGenerator.generateTerrainMesh(noiseMap, meshHeightScalar, meshHeightCurve, meshSimplification), TextureGenerator.colorMapTexture(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    // Clamping values when they are changed in the editor if they go above or below a certain value
    void OnValidate()
    {
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
