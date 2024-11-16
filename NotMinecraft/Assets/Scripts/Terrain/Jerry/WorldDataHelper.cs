using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldDataHelper {
    public static Vector3Int ChunkPosFromBlockCoords(World world, Vector3Int pos) {
        return new Vector3Int {
            x = Mathf.FloorToInt(pos.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(pos.y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(pos.z / (float)world.chunkSize) * world.chunkSize,
        };
    }

    public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPos) {
        int startX = playerPos.x - (world.chunkDrawingRange) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkDrawingRange) * world.chunkSize;
        int endX = playerPos.x + (world.chunkDrawingRange) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkDrawingRange) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new();
        for (int x = startX; x <= endX; x += world.chunkSize) {
            for (int z = startZ; z <= endZ; z += world.chunkSize) {
                Vector3Int chunkPos = ChunkPosFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                if (x >= playerPos.x - world.chunkSize && x <= playerPos.x + world.chunkSize  && z >= playerPos.z - world.chunkSize && z <= playerPos.z + world.chunkSize) {
                    for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkHeight * 2; y -= world.chunkHeight) {
                        chunkPos = ChunkPosFromBlockCoords(world, new Vector3Int(x, y, z));
                        chunkPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }
        return chunkPositionsToCreate;
    }

    public static void RemoveChunkData(World world, Vector3Int pos) {
        world.worldData.chunkDataDictionary.Remove(pos);
    }

    public static void RemoveChunk(World world, Vector3Int pos) {
        if (world.worldData.chunkDictionary.TryGetValue(pos, out ChunkRenderer chunk)) {
            world.RemoveChunk(chunk);
            world.worldData.chunkDictionary.Remove(pos);
        }
    }

    public static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPos) {
        int startX = playerPos.x - (world.chunkDrawingRange + 1) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkDrawingRange + 1) * world.chunkSize;
        int endX = playerPos.x + (world.chunkDrawingRange + 1) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkDrawingRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize) {
            for (int z = startZ; z <= endZ; z += world.chunkSize) {
                Vector3Int chunkPos = ChunkPosFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                if (x >= playerPos.x - world.chunkSize && x <= playerPos.x + world.chunkSize && z >= playerPos.z - world.chunkSize && z <= playerPos.z + world.chunkSize) {
                    for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkHeight * 2; y -= world.chunkHeight) {
                        chunkPos = ChunkPosFromBlockCoords(world, new Vector3Int(x, y, z));
                        chunkDataPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }
        return chunkDataPositionsToCreate;
    }

    public static ChunkRenderer GetChunk(World worldReference, Vector3Int worldPosition) {
        if (worldReference.worldData.chunkDictionary.ContainsKey(worldPosition)) {
            return worldReference.worldData.chunkDictionary[worldPosition];
        }
        return null;
    }

    public static void SetBlock(World worldReference, Vector3Int pos, BlockType blockType) {
        ChunkData chunkData = GetChunkData(worldReference, pos);
        if (chunkData != null) {
            Vector3Int localPosition = Chunk.GetBlockInChunkCoordinates(chunkData, pos);
            Chunk.SetBlock(chunkData, localPosition, blockType);
        }
    }

    public static ChunkData GetChunkData(World worldReference, Vector3Int pos) {
        Vector3Int chunkPosition = ChunkPosFromBlockCoords(worldReference, pos);
        ChunkData containerChunk = null;
        worldReference.worldData.chunkDataDictionary.TryGetValue(chunkPosition, out containerChunk);
        return containerChunk;
    }

    public static List<Vector3Int> GetUnnededData(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded) {
        return worldData.chunkDataDictionary.Keys
            .Where(pos => !allChunkDataPositionsNeeded.Contains(pos)).ToList();
    }

    public static List<Vector3Int> GetUnnededChunks(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded) {
        List<Vector3Int> positionToRemove = new();
        foreach (var pos in worldData.chunkDictionary.Keys.Where(pos => !allChunkPositionsNeeded.Contains(pos))) {
            if (worldData.chunkDictionary.ContainsKey(pos)) {
                positionToRemove.Add(pos);
            }
        }
        return positionToRemove;
    }

    public static List<Vector3Int> SelectPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition) {
        return allChunkPositionsNeeded
            .Where(pos => !worldData.chunkDictionary.ContainsKey(pos))
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    public static List<Vector3Int> SelectDataPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition) {
        return allChunkDataPositionsNeeded
            .Where(pos => !worldData.chunkDataDictionary.ContainsKey(pos))
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }
}
