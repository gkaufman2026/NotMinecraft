using UnityEngine;

public class SurfaceLayerHandler : BlockLayerHandler {
    public BlockType surfaceBlockType;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (y == surfaceHeightNoise) {
            Chunk.SetBlock(data, new Vector3Int(x, y, z), surfaceBlockType);
            return true;
        }
        return false;
    }
}
