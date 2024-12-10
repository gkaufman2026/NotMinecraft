using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData {
    public BlockType[] blocks;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public World world;
    public Vector3Int worldPos;
    private Dictionary<Vector3Int, Bed> bedDictionary = new();

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPos) {
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        this.world = world;
        this.worldPos = worldPos;
        blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
    }

    public Nullable<Vector3Int> GetNearestBed(Vector3Int point)
    {
        Vector3Int blockChunkCoords = point - worldPos;

        var keys = bedDictionary.Keys;
        float distance = chunkSize * 2 * chunkSize * 2 + 1;
        Vector3Int closestKey = Vector3Int.zero;
        bool wasChanged = false;
        foreach (var key in keys)
        {
            Vector3Int vecTo = key - blockChunkCoords;
            float sqrtMag = vecTo.sqrMagnitude;
            if (sqrtMag < distance)
            {
                distance = sqrtMag;
                closestKey = key;
                wasChanged = true;
            }
        }

        if (wasChanged)
        {
            return closestKey + worldPos;
        }

        return null;
    }

    public void AddToBedDictionary(Vector3Int key, Bed value)
    {
        if (!bedDictionary.ContainsKey(key))
        {
            bedDictionary.Add(key, value);
        }
    }

    public Interactable GetInteractable(Vector3Int key)
    {
        if (bedDictionary.ContainsKey(key))
        {
            return bedDictionary[key];
        }

        //Add more interactable checks here for other interactable lists

        return null;
    }
}

