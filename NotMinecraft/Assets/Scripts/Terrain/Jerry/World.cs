using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public GameObject chunkPrefab;
    public Vector3 offset;
    public Vector2Int seedOffset;
    public TerrainGenerator terrainGenerator;

    private GameObject chunksParent;
    private Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new();
    private Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new();

    public GameObject ChunksParent { get => chunksParent; }
    public Dictionary<Vector3Int, ChunkRenderer> ChunkDictionary { get => chunkDictionary; }

    private void Awake() {
        chunksParent = new("Chunks");
    }

    private void FixedUpdate() {
        foreach (ChunkData data in chunkDataDictionary.Values) {
            Vector3 adjustedPosition = data.worldPos + offset;
            chunksParent.transform.position = adjustedPosition;
        }
    }

    public void GenerateWorld() {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values) {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapSizeInChunks; x++) {
            for (int z = 0; z < mapSizeInChunks; z++) {
                Vector3Int chunkPosition = new(x * chunkSize, 0, z * chunkSize);
                ChunkData data = new(chunkSize, chunkHeight, this, chunkPosition);

                ChunkData terrainData = terrainGenerator.GenerateChunkData(data, seedOffset);

                chunkDataDictionary.Add(terrainData.worldPos, terrainData);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values) {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
            chunkObject.transform.parent = chunksParent.transform;
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPos, chunkRenderer);
            chunkRenderer.InitalizeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
    }

    public void ClearWorld() {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values) {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();
    }

    private void GenerateVoxels(ChunkData data) {
        
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int coords) {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, coords);
        chunkDataDictionary.TryGetValue(pos, out ChunkData containerChunk);

        if (containerChunk == null) { return BlockType.AIR; }

        Vector3Int blockInChunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, coords);
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInChunkCoordinates);
    }

    internal ChunkData GetChunkDataFromWorldCords(Vector3Int coords)
    {
        Vector3Int chunkGridCords = coords / chunkSize;
        if (chunkDataDictionary.ContainsKey(chunkGridCords))
        {
            return chunkDataDictionary[chunkGridCords];
        }

        Debug.Log("Could not find chunk");
        return null;
    }

    internal BlockType GetBlockFromWorldCords(Vector3Int coords)
    {
        ChunkData currChunk = GetChunkDataFromWorldCords(coords);
        return GetBlockFromChunkCoordinates(currChunk, coords);
    }

    internal void LoadAdditionalChunksRequest(GameObject player) {
        GenerateWorld();
    }
}
