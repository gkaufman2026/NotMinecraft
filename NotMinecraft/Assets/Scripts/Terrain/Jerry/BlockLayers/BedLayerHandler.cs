using UnityEngine;

public class BedLayerHandler : BlockLayerHandler {
    public BlockType bedBottomType, bedTopType;

    protected override bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (IsSuitableForBed(data, x, y, z)) {
            // Above water level by 1
            Chunk.SetBlock(data, new Vector3Int(x, y + 9, z), bedBottomType);
            Chunk.SetBlock(data, new Vector3Int(x + 1, y + 9, z), bedTopType);
            return true;
        }

        return false;
    }

    private bool IsSuitableForBed(ChunkData data, int x, int y, int z) {
        if (x < 0 || x >= data.chunkSize - 1 || z < 0 || z >= data.chunkHeight - 1) {
            return false;
        }
        
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                Vector3Int chunkCoordinates = new(x + i, y , z + j);
                BlockType type = Chunk.GetBlockFromChunkCoordinates(data, chunkCoordinates);
                Debug.Log(type);

                if (type != BlockType.AIR) {
                    return false;
                }
            }
        }

        bool hasSpaceOnXAxis = !Chunk.IsSolidBlock(data, x + 2, y, z) && !Chunk.IsSolidBlock(data, x + 2, y, z + 1);
        bool hasSpaceOnZAxis = !Chunk.IsSolidBlock(data, x, y, z + 2) && !Chunk.IsSolidBlock(data, x + 1, y, z + 2);

        return hasSpaceOnXAxis || hasSpaceOnZAxis;
    }

}
