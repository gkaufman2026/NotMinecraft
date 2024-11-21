using UnityEngine;

public static class NoiseOptions
{
    public static float RemapValue(float value, float min, float max, float finalMin, float finalMax) {
        return finalMin + (value - min) * (finalMax - finalMin) / (max - min);
    }

    public static float RemapValue(float value, float min, float max) {
        return min + (value - 0) * (max - min) / (1 - 0);
    }

    public static int ConvertMappedValueToInt(float value, float min, float max) {
        return (int)RemapValue(value, min, max);
    }

    public static float Redistribute(float noise, NoiseSettings settings) {
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings) {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        float total = 0, frequency = 1, amplitude = 1, amplitudeSum = 0;  
        for (int i = 0; i < settings.octaves; i++) {
            total += Mathf.PerlinNoise((settings.offset.x + settings.worldOffset.x + x) * frequency, (settings.offset.y + settings.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= settings.persistance;
            frequency *= 2;
        }

        return total / amplitudeSum;
    }
}
