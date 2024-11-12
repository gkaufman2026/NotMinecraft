using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
public class AlexMapGenerator : MonoBehaviour
{
    public enum DrawingMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawingMode drawingMode;

    public Noise.NormalizeMode normalizeMode;

    // 241 is choosen here because of Unity's mesh vertice limit of 65025, 240 is divisable by 2,4,6,8,10,and 12
    // making it optimal for chunk threading, 241 is choosen because our actual size will be the size - 1
    // When looping through the map, we are incrementing by i, creating a vertex at each point
    // The created mesh can be simplified by making i > 1, doing this will decrease the totla number of verticies
    // however i must be a factor of the map's widthm if i is not a factor, we will leave the scope of the map array
    // since we are looping through the width and height of the map
    // width and height must both be less than 255 since 255^2 = 65025
    public const int mapChunkSize = 241;

    [Range(0, 6)]
    public int lod;
    public float noiseScale;

    public int octaves;
    // Creates a slider in the editor for persistence
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightScalar;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<AlexMeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<AlexMeshData>>();

    // ImGui Ui
    private void OnGUI()
    {
        float octavesF = (float)octaves;
        string seedString = seed.ToString();

        GUI.BeginGroup(new Rect(25, 25, 300, 325));

        GUI.Box(new Rect(0, 0, 200, 500), "Mesh Generation Options");
        GUI.Label(new Rect(10, 20, 200, 500), "Mesh will auto update when values are changed!");

        GUI.Label(new Rect(10, 60, 120, 30), "Noise Scale: " + Mathf.Round(noiseScale * 100) / 100.0);
        noiseScale = GUI.HorizontalSlider(new Rect(10, 80, 100, 30), noiseScale, 1.0f, 100.0f);

        GUI.Label(new Rect(10, 92, 120, 30), "Octaves: " + octaves);
        octavesF = GUI.HorizontalSlider(new Rect(10, 114, 100, 30), octavesF, 1.0f, 50.0f);
        octaves = (int)octavesF;

        GUI.Label(new Rect(10, 124, 120, 30), "Persistance: " + Mathf.Round(persistence * 100) / 100.0);
        persistence = GUI.HorizontalSlider(new Rect(10, 144, 100, 30), persistence, 0.0f, 1.0f);

        GUI.Label(new Rect(10, 154, 120, 30), "Lacunarity: " + Mathf.Round(lacunarity * 100) / 100.0);
        lacunarity = GUI.HorizontalSlider(new Rect(10, 174, 100, 30), lacunarity, 0.0f, 1.92f);

        GUI.Label(new Rect(10, 186, 120, 30), "Seed: ");
        seedString = GUI.TextField(new Rect(80, 188, 60, 20), seedString);
        seed = int.Parse(seedString);

        GUI.Label(new Rect(10, 210, 120, 30), "X Offset: " + Mathf.Round(offset.x * 100) / 100.0);
        offset.x = GUI.HorizontalSlider(new Rect(10, 230, 100, 30), offset.x, -100.0f, 100.0f);

        GUI.Label(new Rect(10, 242, 120, 30), "Y Offset: " + Mathf.Round(offset.y * 100) / 100.0);
        offset.y = GUI.HorizontalSlider(new Rect(10, 262, 100, 30), offset.y, -100.0f, 100.0f);

        GUI.Label(new Rect(10, 274, 200, 30), "Mesh Height Scalar: " + Mathf.Round(meshHeightScalar * 100) / 100.0);
        meshHeightScalar = GUI.HorizontalSlider(new Rect(10, 294, 100, 30), meshHeightScalar, -100.0f, 100.0f);

        GUI.EndGroup();
    }

    // Draws the type of map based upon what is specified within the editor
    public void drawMapInEditor()
    {
        MapData mapData = generateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawingMode == DrawingMode.NoiseMap)
        {
            display.drawTexture(TextureGenerator.heightMapTexture(mapData.heightMap));
        }
        else if (drawingMode == DrawingMode.ColorMap)
        {
            display.drawTexture(TextureGenerator.colorMapTexture(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawingMode == DrawingMode.Mesh)
        {
            display.drawMesh(MeshGenerator.generateTerrainMesh(mapData.heightMap, meshHeightScalar, meshHeightCurve, lod), TextureGenerator.colorMapTexture(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    // Threading works by passing in methods as a varible where map data is sent and recived
    // between multiple methods, dividing up the work that is being done across multiple processes

    // This function starts the thread that will be used
    // the callback parameter refers to the mapDataRecived method
    public void requestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            mapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    // Gets the map data and adds it to a queue
    void mapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = generateMapData(center);

        // locking makes it so when one thread reaches this point no other thread can execute this code
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    // This is the same threading process as the maps but instead for the meshes that are created
    public void requestMeshData(MapData mapData, int lod, Action<AlexMeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            meshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    // Gets the mesh data and adds it to a queue
    void meshDataThread(MapData mapData, int lod, Action<AlexMeshData> callback)
    {
        AlexMeshData meshData = MeshGenerator.generateTerrainMesh(mapData.heightMap, meshHeightScalar, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<AlexMeshData>(callback, meshData));
        }
    }

    // Recives info from the map data and mesh data queues and removes it from the queue
    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<AlexMeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    // Generates the noise and color maps
    MapData generateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, center + offset, normalizeMode);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    // When true, means region of the currentHeight of the noise map is found 
                    if (currentHeight >= regions[i].height)
                    {
                        // Sets the color map at index to the corresponding region color
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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

    struct MapThreadInfo<T>
    {
        public Action<T> callback;
        public T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
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

public struct MapData
{
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
