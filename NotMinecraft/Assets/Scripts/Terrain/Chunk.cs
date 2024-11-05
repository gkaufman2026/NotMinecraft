using System;
using UnityEngine;

public static class Chunk {
    public static void LoopThroughBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform) {
        for (int i = 0; i < chunkData.blocks.Length; i++) {
            Vector3Int pos = GetPostitionFromIndex(chunkData, i);
            actionToPerform(pos.x, pos.y, pos.z);
        }
    }

    private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int i) {
        return new Vector3Int {
            x =  i % chunkData.chunkSize,
            y = (i / chunkData.chunkSize) % chunkData.chunkHeight,
            z =  i / (chunkData.chunkSize * chunkData.chunkHeight)
        };
    }

    private static bool IsInRange(ChunkData chunkData, int axis) {
        return !(axis < 0 || axis >= chunkData.chunkSize);
    }

    private static bool InRangeHeight(ChunkData chunkData, int yCoord) {
        return !(yCoord < 0 || yCoord >= chunkData.chunkHeight);
    }

    private static bool IsVectorInRange(ChunkData chunkData, Vector3Int coords) {
        return IsInRange(chunkData, coords.x) && InRangeHeight(chunkData, coords.y) && IsInRange(chunkData, coords.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates) {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z) {
        if (IsVectorInRange(chunkData, new Vector3Int(x,y,z))) {
            int index = GetIndexFromPosition(chunkData, new Vector3Int(x,y,z));
            return chunkData.blocks[index];
        }
        return chunkData.world.GetBlockFromChunkCoordinates(chunkData, new Vector3Int(chunkData.worldPos.x + x, chunkData.worldPos.y + y, chunkData.worldPos.z + z));
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block) {
        if (IsInRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && IsInRange(chunkData, localPosition.z)) {
            int index = GetIndexFromPosition(chunkData, localPosition);
            chunkData.blocks[index] = block;
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, Vector3Int coords) {
        return coords.x + chunkData.chunkSize * coords.y + chunkData.chunkSize * chunkData.chunkHeight * coords.z;
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos) {
        return new Vector3Int {
            x = pos.x - chunkData.worldPos.x,
            y = pos.y - chunkData.worldPos.y,
            z = pos.z - chunkData.worldPos.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData) {
        MeshData meshData = new(true);
        LoopThroughBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, new Vector3Int(x, y, z), meshData, chunkData.blocks[GetIndexFromPosition(chunkData, new Vector3Int(x, y, z))]));
        return meshData;
    }

    internal static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int coords) {
        return new Vector3Int {
            x = Mathf.FloorToInt(coords.x / (float) world.chunkSize)   * world.chunkSize,
            y = Mathf.FloorToInt(coords.y / (float) world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(coords.z / (float) world.chunkSize)   * world.chunkSize
        };
    }
}