using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour {
    // Each block in world is equal to threshold of 3
    public int waterThreshold = 50, sandThreshold = 6;
    public NoiseSettings biomeNoiseSetting;
    public BlockLayerHandler startLayer;
    public bool useDomainWarping;
    public List<BlockLayerHandler> additionalLayers;

    private DomainWarping domainWarping;

    private void Awake() {
        domainWarping = GetComponent<DomainWarping>();
    }

    public ChunkData ProcessChunkPerColumn(ChunkData data, int x, int z, Vector2Int offset) {
        biomeNoiseSetting.worldOffset = offset;
        int groundPos = GetSurfaceHeightNoise(data.worldPos.x + x, data.worldPos.z + z, data.chunkHeight);

        for (int y = 0; y < data.chunkHeight; y++) {
            startLayer.Handle(data, x, y, z, groundPos, offset);
        }

        foreach (var layer in additionalLayers) {
            layer.Handle(data, x, data.worldPos.y, z, groundPos, offset);
        }

        return data;
    }

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight) {
        float terrainHeight = !useDomainWarping ? NoiseOptions.OctavePerlin(x, z, biomeNoiseSetting) : domainWarping.GenerateDomainNoise(x, z, biomeNoiseSetting);
        terrainHeight = NoiseOptions.Redistribute(terrainHeight, biomeNoiseSetting);
        return NoiseOptions.ConvertMappedValueToInt(terrainHeight, 0, chunkHeight);
    }
}
