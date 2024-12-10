using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public GameObject chunkPrefab;
    public Vector2Int seedOffset;
    public TerrainGenerator terrainGenerator;
    public GameManager gameManager;

    private GameObject chunksParent;
    private Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new();
    private Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new();

    public GameObject ChunksParent { get => chunksParent; }
    public Dictionary<Vector3Int, ChunkRenderer> ChunkDictionary { get => chunkDictionary; }

    private void Awake() {
        chunksParent = new("Chunks");
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
        //gameManager.SpawnPlayer();
    }

    public void ClearWorld() {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values) {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int coords) { 
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, coords);
        chunkDataDictionary.TryGetValue(pos, out ChunkData containerChunk);

        if (containerChunk == null) { return BlockType.AIR; }

        Vector3Int blockInChunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, coords);
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInChunkCoordinates);
    }

    internal ChunkData GetChunkDataFromWorldCoords(Vector3Int coords) {
        Vector3Int chunkGridCords = (coords / chunkSize) * chunkSize;
        if (chunkDataDictionary.ContainsKey(chunkGridCords)) {
            return chunkDataDictionary[chunkGridCords];
        }

        Debug.Log("Could not find chunk");
        return null;
    }

    internal BlockType GetBlockFromWorldCoords(World world, Vector3Int coords) {
        Vector3Int chunkGridCords = Chunk.ChunkPositionFromBlockCoords(world, coords);
        if (chunkDataDictionary.ContainsKey(chunkGridCords)) {
            return world.GetBlockFromChunkCoordinates(chunkDataDictionary[chunkGridCords], coords);
        }

        Debug.Log("Could not find block in chunk");
        return BlockType.AIR;
    }

    internal Nullable<Vector3Int> GetNearestBedInChunk(Vector3Int point)
    {
        //Finds current chunk entity is in
        ChunkData data = GetChunkDataFromWorldCoords(point);
        ChunkData data2;
        ChunkData data3;
        ChunkData data4;
        Vector2Int direction = Vector2Int.zero;
        List<Nullable<Vector3Int>> beds = new List<Nullable<Vector3Int>>();

        Vector3Int posInChunk = point - data.worldPos;
        if (posInChunk.x < data.chunkSize / 2)
        {
            direction.x = -1;
        }
        else
        {
            direction.x = 1;
        }

        if (posInChunk.z < data.chunkSize / 2)
        {
            direction.y = -1;
        }
        else
        {
            direction.y = 1;
        }

        data2 = GetChunkDataFromWorldCoords(point + new Vector3Int(chunkSize * direction.x, 0, 0));
        data3 = GetChunkDataFromWorldCoords(point + new Vector3Int(0, 0, chunkSize * direction.y));
        data4 = GetChunkDataFromWorldCoords(point + new Vector3Int(chunkSize * direction.x, 0, chunkSize * direction.y));

        beds.Add(data.GetNearestBed(point));
        beds.Add(data2.GetNearestBed(point));
        beds.Add(data3.GetNearestBed(point));
        beds.Add(data4.GetNearestBed(point));

        float distance = chunkSize * 2 * chunkSize * 2 + 1;
        Vector3Int closestBed = Vector3Int.zero;
        bool wasChanged = false;
        foreach (var bed in beds)
        {
            if (bed != null)
            {
                Vector3Int bedNew = (Vector3Int)bed;
                Vector3Int vecTo = bedNew - point;
                float sqrtMag = vecTo.sqrMagnitude;
                if (sqrtMag < distance)
                {
                    distance = sqrtMag;
                    closestBed = bedNew;
                    wasChanged = true;
                }
            }
        }

        if (wasChanged)
        {
            return closestBed;
        }

        return null;
    }
        
}
