using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    // Each block in world is equal to threshold of 3
    public int waterThreshold = 50, stoneThreshold = 21, sandThreshold = 6; 
    public float noiseScale = 0.05f;
    public GameObject chunkPrefab;
    public Vector3 offset;

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
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPos, data);
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
        for (int x = 0; x < data.chunkSize; x++) {
            for (int z = 0; z < data.chunkSize; z++) {
                float noiseValue = Mathf.PerlinNoise((data.worldPos.x + x) * noiseScale, (data.worldPos.z + z) * noiseScale);
                int groundPos = Mathf.RoundToInt(noiseValue * chunkHeight);

                for (int y = 0; y < data.chunkHeight; y++) {
                    BlockType voxelType;
                    if (y > groundPos) {
                        // ABOVE GROUND
                        voxelType = y < waterThreshold ? BlockType.WATER : BlockType.AIR;
                    } else if (y == groundPos) {
                        // GRASS ABOVE WATER, SAND UNDERWATER
                        voxelType = y >= waterThreshold ? BlockType.GRASS : BlockType.SAND;
                    } else if (y > groundPos - 4 && y < groundPos) {
                        // DIRT LAYERS BELOW SURFACE, ONLY IF NOT UNDER WATER
                        if (y >= waterThreshold) {
                            voxelType = BlockType.DIRT;
                        } else {
                            voxelType = (y > groundPos - sandThreshold) ? BlockType.SAND : BlockType.STONE;
                        }
                    } else if (y <= waterThreshold && y > groundPos - sandThreshold) {
                        // SAND NEAR WATER, BENEATH SURFACE, LIMITED TO WATER SECTIONS
                        voxelType = BlockType.SAND;
                    } else {
                        voxelType = BlockType.STONE;
                    }

                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int coords) {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, coords);
        chunkDataDictionary.TryGetValue(pos, out ChunkData containerChunk);

        if (containerChunk == null) { return BlockType.AIR; }

        Vector3Int blockInChunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, coords);
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInChunkCoordinates);
    }
}
