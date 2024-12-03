using UnityEngine;

public class BedLayerHandler : BlockLayerHandler {
    public BlockType[] bedSurfaceTypes;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {         
        // Check if above ground and there is at least a two block length available 
        
        return false;
    }
}