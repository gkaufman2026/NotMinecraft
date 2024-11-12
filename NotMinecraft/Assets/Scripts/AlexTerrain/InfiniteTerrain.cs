using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    public static float maxViewDistance;
    public LodInfo[] detailLevels;

    public Transform player;
    public Material mapMaterial;

    public static Vector2 playerPosition;
    static AlexMapGenerator mapGenerator;

    int chunkSize;
    int chunksVisible;

    // Vector for chunk coordinate and terrainChunk as the value
    Dictionary<Vector2, TerrainChunk> chunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> previouslyVisibleTerrainChunks = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<AlexMapGenerator>();

        // Sets max view distance to the last visible distance threshold
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;

        chunkSize = AlexMapGenerator.mapChunkSize - 1;

        // How many times we can divide the chunk size into view distance
        chunksVisible = Mathf.RoundToInt(maxViewDistance / chunkSize);

        updateVisibleChunks();
    }

    // Gets the current player position and calls update visible chunks
    void Update()
    {
        playerPosition = new Vector2(player.position.x, player.position.z);
        updateVisibleChunks();
    }

    // Updates all chunks within the viewable range and then each of those visible chunks after the update
    // are added to the previouslyVisibleTerrainChunks list
    void updateVisibleChunks()
    {
        for (int i = 0; i < previouslyVisibleTerrainChunks.Count; i++)
        {
            previouslyVisibleTerrainChunks[i].setVisibility(false);
        }

        previouslyVisibleTerrainChunks.Clear();

        int chunkXCord = Mathf.RoundToInt(playerPosition.x / chunkSize);
        int chunkYCord = Mathf.RoundToInt(playerPosition.y / chunkSize);

        for (int y = -chunksVisible; y <= chunksVisible; y++)
        {
            for (int x = -chunksVisible; x <= chunksVisible; x++)
            {
                Vector2 viewedChunkCord = new Vector2(chunkXCord + x, chunkYCord + y);

                if (chunkDictionary.ContainsKey(viewedChunkCord))
                {
                    chunkDictionary[viewedChunkCord].updateChunk();
                } else {
                    chunkDictionary.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;
        bool mapDataReceived;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        int previouslodIndex = -1;

        // Arrays for the simplification values and for the simplified meshes
        LodInfo[] detailLevels;
        LodMesh[] LodMeshes;

        public TerrainChunk(Vector2 cord, int size, LodInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = cord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");

            // Add component reutrns the componenet it adds so declarations are possible
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = position3;
            meshObject.transform.parent = parent;

            // Set to invisible by default so the update can handle the change if it is needed
            setVisibility(false);

            LodMeshes = new LodMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                LodMeshes[i] = new LodMesh(detailLevels[i].lod, updateChunk);
            }

            mapGenerator.requestMapData(position, onMapDataReceived);
        }

        // When map data is recieved the map texture is updated
        void onMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.colorMapTexture(mapData.colorMap, AlexMapGenerator.mapChunkSize, AlexMapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
        }

        // Updates chunks with the appropriate detail level based upon the players distance from them
        public void updateChunk()
        {
            if (mapDataReceived)
            {
                float playerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
                bool isVisible = playerDistanceFromNearestEdge <= maxViewDistance;

                if (isVisible)
                {
                    int lodIndex = 0;

                    // Last element is not included because the isVisible bool will be false because
                    // playerDistanceFromNearestEdge will be greater than max view distance
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (playerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                        {
                            lodIndex = i + 1;
                        } else {
                            break;
                        }
                    }

                    if (lodIndex != previouslodIndex)
                    {
                        LodMesh LodMesh = LodMeshes[lodIndex];

                        if (LodMesh.hasMesh)
                        {
                            previouslodIndex = lodIndex;
                            meshFilter.mesh = LodMesh.mesh;
                        }
                        else if (!LodMesh.requestedMesh)
                        {
                            LodMesh.meshRequest(mapData);
                        }
                    }

                    previouslyVisibleTerrainChunks.Add(this);
                }

                setVisibility(isVisible);
            }
            
        }

        public void setVisibility(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }

    // Gets quaility meshes based upon distance from player
    class LodMesh
    {
        public Mesh mesh;
        public bool requestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LodMesh(int lod, System.Action updateCallBack)
        {
            this.lod = lod;
            this.updateCallback = updateCallBack;
        }

        void onMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.createMesh();
            hasMesh = true;

            updateCallback();
        }

        public void meshRequest(MapData mapData)
        {
            requestedMesh = true;
            mapGenerator.requestMeshData(mapData, lod, onMeshDataReceived);
        }
    }

    // Struct defining the level of mesh simplification and distance that simplification will occur
    [System.Serializable]
    public struct LodInfo
    {
        public int lod;
        public float visibleDistanceThreshold;
    }
}
