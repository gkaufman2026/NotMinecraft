using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour {
    public struct WorldGenerationData {
        public List<Vector3Int> chunkPosToCreate, chunkDataPosToCreate, chunkPosToRemove, chunkDataToRemove;
    }

    public struct WorldData {
        public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
        public int chunkSize, chunkHeight;
    }

    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100, chunkDrawingRange = 8;
    public GameObject chunkPrefab;
    public Vector3 offset;
    public Vector2Int seedOffset;
    public TerrainGenerator terrainGenerator;

    private GameObject chunksParent;

    public GameObject ChunksParent { get => chunksParent; }
    public Dictionary<Vector3Int, ChunkRenderer> ChunkDictionary { get => worldData.chunkDictionary; }

    public UnityEvent OnWorldCreated, OnNewChunksGenerated;
    public WorldData worldData { get; private set; }

    private void Awake() {
        chunksParent = new("Chunks");
        worldData = new WorldData {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
        };
    }

    private void FixedUpdate() {
        foreach (ChunkData data in worldData.chunkDataDictionary.Values) {
            Vector3 adjustedPosition = data.worldPos + offset;
            chunksParent.transform.position = adjustedPosition;
        }
    }

    public void GenerateWorld() {
        GenerateWorld(Vector3Int.zero);
    }

    private void GenerateWorld(Vector3Int position) {
        WorldGenerationData worldGenerationData = GetChunksVisibleToPlayer(position);

        foreach (Vector3Int pos in worldGenerationData.chunkPosToRemove) {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach (Vector3Int pos in worldGenerationData.chunkDataToRemove) {
            WorldDataHelper.RemoveChunkData(this, pos);
        }

        foreach (var pos in worldGenerationData.chunkDataPosToCreate) {
            ChunkData data = new(chunkSize, chunkHeight, this, pos);
            ChunkData newData = terrainGenerator.GenerateChunkData(data, seedOffset);
            worldData.chunkDataDictionary.Add(pos, newData);
        }

        foreach (var pos in worldGenerationData.chunkPosToCreate) {
            ChunkData data = worldData.chunkDataDictionary[pos];
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
            chunkObject.transform.parent = chunksParent.transform;
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            worldData.chunkDictionary.Add(data.worldPos, chunkRenderer);
            chunkRenderer.InitalizeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }

        OnWorldCreated?.Invoke();
    }

    internal void RemoveChunk(ChunkRenderer chunk) {
        chunk.gameObject.SetActive(false);
    }

    private WorldGenerationData GetChunksVisibleToPlayer(Vector3Int playerPos) {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPos);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPos);

        List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPos);
        List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPos);

        List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnnededChunks(worldData, allChunkPositionsNeeded);
        List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnnededData(worldData, allChunkDataPositionsNeeded);

        WorldGenerationData data = new() {
            chunkPosToCreate = chunkPositionsToCreate,
            chunkDataPosToCreate = chunkDataPositionsToCreate,
            chunkPosToRemove = chunkPositionsToRemove,
            chunkDataToRemove = chunkDataToRemove,
        };
        return data;

    }

    public void ClearWorld() {
        worldData.chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in worldData.chunkDictionary.Values) {
            Destroy(chunk.gameObject);
        }
        worldData.chunkDictionary.Clear();
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int coords) {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, coords);
        worldData.chunkDataDictionary.TryGetValue(pos, out ChunkData containerChunk);

        if (containerChunk == null) { return BlockType.AIR; }

        Vector3Int blockInChunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, coords);
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInChunkCoordinates);
    }

    internal ChunkData GetChunkDataFromWorldCords(Vector3Int coords) {
        Vector3Int chunkGridCords = coords / chunkSize;
        if (worldData.chunkDataDictionary.ContainsKey(chunkGridCords))
        {
            return worldData.chunkDataDictionary[chunkGridCords];
        }

        Debug.Log("Could not find chunk");
        return null;
    }

    internal BlockType GetBlockFromWorldCords(Vector3Int coords) {
        ChunkData currChunk = GetChunkDataFromWorldCords(coords);
        return GetBlockFromChunkCoordinates(currChunk, coords);
    }

    internal void LoadAdditionalChunksRequest(GameObject player) {
        GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }
}
