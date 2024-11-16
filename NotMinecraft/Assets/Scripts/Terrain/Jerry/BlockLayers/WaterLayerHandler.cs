using UnityEngine;

public class WaterLayerHandler : BlockLayerHandler {
    public int waterLevel = 1;
    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (y > surfaceHeightNoise && y <= waterLevel) {
            Chunk.SetBlock(data, new Vector3Int(x, y, z), BlockType.WATER);

            if (y == surfaceHeightNoise + 1) {
                Chunk.SetBlock(data, new Vector3Int(x, surfaceHeightNoise, z), BlockType.SAND);
            }
            return true;
        }
        return false;
    }
}
