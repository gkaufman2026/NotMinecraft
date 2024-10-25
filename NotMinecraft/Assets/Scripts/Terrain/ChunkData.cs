using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData {
    public BlockType[] blocks;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public World world;
    public Vector3Int worldPos;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPos) {
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        this.world = world;
        this.worldPos = worldPos;
        blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
    }
}

