using System;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    // Each block in world is equal to threshold of 3
    public int waterThreshold = 50, stoneThreshold = 21, sandThreshold = 6;
    public float noiseScale = 0.05f;

    public ChunkData ProcessChunkPerColumn(ChunkData data, int x, int z) {
        float noiseValue = Mathf.PerlinNoise((data.worldPos.x + x) * noiseScale, (data.worldPos.z + z) * noiseScale);
        int groundPos = Mathf.RoundToInt(noiseValue * data.chunkHeight);

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
        return data;
    }
}
