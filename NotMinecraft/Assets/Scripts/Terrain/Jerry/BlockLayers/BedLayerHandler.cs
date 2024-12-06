using UnityEngine;

public class BedLayerHandler : BlockLayerHandler {
    public BlockType bedBottomType, bedTopType;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) { 
        if (Chunk.GetBlockFromChunkCoordinates(data, new Vector3Int(5, 8, 4)) == BlockType.GRASS) {
            Chunk.SetBlock(data, new Vector3Int(5, 9, 4), bedBottomType);
            Chunk.SetBlock(data, new Vector3Int(6, 9, 4), bedTopType);
            return true;
        }
        return false;
    }
}
