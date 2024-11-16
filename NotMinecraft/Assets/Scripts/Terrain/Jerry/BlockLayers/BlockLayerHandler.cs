using UnityEngine;

public abstract class BlockLayerHandler : MonoBehaviour
{
    [SerializeField] private BlockLayerHandler Next;

    public bool Handle(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset) {
        if (TryHandling(data, x, y, z, surfaceHeightNoise, seedOffset)) {
            return true;
        }

        if (Next != null) {
            return Next.Handle(data, x, y, z, surfaceHeightNoise, seedOffset);
        }

        return false;
    }

    protected abstract bool TryHandling(ChunkData data, int x, int y, int z, int surfaceHeightNoise, Vector2Int seedOffset);
}
