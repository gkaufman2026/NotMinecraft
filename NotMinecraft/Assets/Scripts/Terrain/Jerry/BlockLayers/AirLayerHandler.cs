using UnityEngine;

public class AirLayerHandler : BlockLayerHandler {
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (y > surfaceHeightNoise) {
            Chunk.SetBlock(data, new Vector3Int(x, y, z), BlockType.AIR);
            return true;
        }
        return false;
    }
}
