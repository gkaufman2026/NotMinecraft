using System;
using UnityEngine;

// Helper Class For Chunk Functions 
public static class Chunk {
    // Will parse through each block of chunk and performs an Action per the position
    public static void LoopThroughChunk(ChunkData chunkData, Action<int, int, int> actionToPerform) {
        for (int i = 0; i < chunkData.blocks.Length; i++) {
            Vector3Int pos = GetPostitionFromIndex(chunkData, i);
            actionToPerform(pos.x, pos.y, pos.z);
        }
    }

    // Grabs the position of the chunk based off the index provided
    private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int i) {
        return new Vector3Int {
            x =  i % chunkData.chunkSize,
            y = (i / chunkData.chunkSize) % chunkData.chunkHeight,
            z =  i / (chunkData.chunkSize * chunkData.chunkHeight)
        };
    }

    // Checks if the chunk is in range depending on the axis in correlation if its less than 0 or axis is greater than or equal to the chunk size
    private static bool IsInRange(ChunkData chunkData, int axis) {
        return !(axis < 0 || axis >= chunkData.chunkSize);
    }

    // Checks the y axis, like above function above
    private static bool InRangeHeight(ChunkData chunkData, int yCoord) {
        return !(yCoord < 0 || yCoord >= chunkData.chunkHeight);
    }

    // Uses the two above functions to check if x, y and z axis are in range
    private static bool IsVectorInRange(ChunkData chunkData, Vector3Int coords) {
        return IsInRange(chunkData, coords.x) && InRangeHeight(chunkData, coords.y) && IsInRange(chunkData, coords.z);
    }

    // Gets block depending within the the chunk coords
    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates) {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    // Checking if chunk is within range and returns chunk based off of the index
    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z) {
        if (IsVectorInRange(chunkData, new Vector3Int(x,y,z))) {
            int index = GetIndexFromPosition(chunkData, new Vector3Int(x,y,z));
            return chunkData.blocks[index];
        }
        return chunkData.world.GetBlockFromChunkCoordinates(chunkData, new Vector3Int(chunkData.worldPos.x + x, chunkData.worldPos.y + y, chunkData.worldPos.z + z));
    }

    public static bool IsSolidBlock(ChunkData data, int x, int y, int z) {
        return BlockDataManager.textureList[GetBlockFromChunkCoordinates(data, x, y, z)].isSolid;
    }

    public static bool HasCollider(ChunkData data, int x, int y, int z) {
        return BlockDataManager.textureList[GetBlockFromChunkCoordinates(data, x, y, z)].hasCollider;
    }

    // Sets the block based off the chunk's position
    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block) {
        if (IsVectorInRange(chunkData, localPosition)) {
            int index = GetIndexFromPosition(chunkData, localPosition);
            chunkData.blocks[index] = block;
        }
    }

    // Gets chunk index from position 
    private static int GetIndexFromPosition(ChunkData chunkData, Vector3Int coords) {
        return coords.x + chunkData.chunkSize * coords.y + chunkData.chunkSize * chunkData.chunkHeight * coords.z;
    }

    // Returns block vector from Chunk Coords
    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos) {
        return new Vector3Int {
            x = pos.x - chunkData.worldPos.x,
            y = pos.y - chunkData.worldPos.y,
            z = pos.z - chunkData.worldPos.z
        };
    }

    // Gets the chunk mesh data by parsing through the chunk using a lambda 
    public static MeshData GetChunkMeshData(ChunkData chunkData) {
        MeshData meshData = new(true);
        LoopThroughChunk(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, new Vector3Int(x, y, z), meshData, chunkData.blocks[GetIndexFromPosition(chunkData, new Vector3Int(x, y, z))]));
        return meshData;
    }

    // Returns chunk position from the block coordinates 
    internal static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int coords) {
        return new Vector3Int {
            x = Mathf.FloorToInt(coords.x / (float) world.chunkSize)   * world.chunkSize,
            y = Mathf.FloorToInt(coords.y / (float) world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(coords.z / (float) world.chunkSize)   * world.chunkSize
        };
    }
}