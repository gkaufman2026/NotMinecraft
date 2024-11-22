using UnityEngine;

public class StoneLayerHandler : BlockLayerHandler {
    [Range(0, 1)]
    public float stoneThreshold = 0.5f;

    [SerializeField]
    private NoiseSettings stoneNoiseSettings;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (data.worldPos.y > surfaceHeightNoise) {
            return false;
        }

        stoneNoiseSettings.worldOffset = seedOffset;
        float stoneNoise = NoiseOptions.OctavePerlin(data.worldPos.x + x, data.worldPos.z + z, stoneNoiseSettings);
        int endPosition = surfaceHeightNoise;
        if (data.worldPos.y < 0) {
            endPosition = data.worldPos.y + data.chunkHeight;
        }

        if (stoneNoise > stoneThreshold) {
            for (int i = data.worldPos.y; i <= endPosition; i++) {
                Chunk.SetBlock(data, new Vector3Int(x, i, z), BlockType.STONE);
            }
            return true;
        }
        return false;
    }
}