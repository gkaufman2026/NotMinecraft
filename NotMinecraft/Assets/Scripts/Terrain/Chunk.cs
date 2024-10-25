using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Chunk
{
    public static void LoopThroughBlocks(ChunkData data, Action<int, int, int> actioPerform)
    {
        for (int i = 0; i < data.blocks.Length; i++)
        {
            var pos = GetPositionFromIndex(data, i);
            actioPerform(pos.x, pos.y, pos.z);
        }
    }

    private static Vector3Int GetPositionFromIndex(ChunkData data, int i)
    {
        int x = i % data.chunkSize;
        int y = (i / data.chunkSize) % data.chunkHeight;
        int z = i / (data.chunkSize * data.chunkHeight);
        return new Vector3Int(x, y, z);
    }

    private static bool InRange(ChunkData chunkData, int axisCoordinate)
    {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
        {
            return false;
        }

        return true;
    }

    private static bool InRangeHeight(ChunkData chunkData, int yCoord)
    {
        if (yCoord < 0 || yCoord >= chunkData.chunkHeight)
        {
            return false;
        }

        return true;
    }

    public static BlockType GetBlockFromChunkCoord(ChunkData data, Vector3Int coord)
    {
        return GetBlockFromChunkCoordinates(data, coord);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int coord)
    {
        if (InRange(chunkData, coord.x) && InRangeHeight(chunkData, coord.y) && InRange(chunkData, coord.z))
        {
            int index = GetIndexFromPosition(chunkData, coord);
            return chunkData.blocks[index];
        }

        return BlockType.NOTHING;
        //return chunkData.world.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPos.x + coord.x, chunkData.worldPos.y + coord.y, chunkData.worldPos.z + coord.z);
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
        {
            int index = GetIndexFromPosition(chunkData, localPosition);
            chunkData.blocks[index] = block;
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, Vector3Int pos)
    {
        return pos.x + chunkData.chunkSize * pos.y + chunkData.chunkSize * chunkData.chunkHeight * pos.z;
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPos.x,
            y = pos.y - chunkData.worldPos.y,
            z = pos.z - chunkData.worldPos.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new(true);
        //LoopThroughBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, new Vector3Int(x, y, z))]));

        return meshData;
    }
}
