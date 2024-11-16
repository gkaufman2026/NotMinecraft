using System;
using UnityEngine;

public class DomainWarping : MonoBehaviour {
    public NoiseSettings noiseDomainX, noiseDomainY;
    public Vector2Int amplitude = new(20, 20);

    public float GenerateDomainNoise(int x, int z, NoiseSettings defaultSettings) {
        Vector2 domainOffset = GenerateDomainOffset(x, z);
        return NoiseOptions.OctavePerlin(x + domainOffset.x, z + domainOffset.y, defaultSettings);
    }

    public Vector2 GenerateDomainOffset(int x, int z) {
        return new Vector2(
            NoiseOptions.OctavePerlin(x, z, noiseDomainX) * amplitude.x,
            NoiseOptions.OctavePerlin(x, z, noiseDomainY) * amplitude.y
        );
    }

    public Vector2Int GenerateDomainOffsetInt(int x, int z) {
        return Vector2Int.RoundToInt(GenerateDomainOffset(x, z));
    }
}
