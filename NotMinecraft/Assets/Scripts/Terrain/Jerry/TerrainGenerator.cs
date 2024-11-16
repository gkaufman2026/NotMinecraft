using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;

    public ChunkData GenerateChunkData(ChunkData data, Vector2Int offset) {
        for (int x = 0; x < data.chunkSize; x++) {
            for (int z = 0; z < data.chunkSize; z++) {
                data = biomeGenerator.ProcessChunkPerColumn(data, x, z, offset);
            }
        }
        return data;
    }
}
