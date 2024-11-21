using UnityEngine;

public class UndergroundLayerHandler : BlockLayerHandler {
    public BlockType undergroundBlockType;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (y < surfaceHeightNoise) {
            Chunk.SetBlock(data, new Vector3Int(x, y, z), undergroundBlockType);
            return true;
        }
        return false;
    }
}
