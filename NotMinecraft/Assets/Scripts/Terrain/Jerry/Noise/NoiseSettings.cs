using UnityEngine;

[CreateAssetMenu(fileName = "Noise Settings", menuName = "NotMinecraft/Noise Settings")]
public class NoiseSettings : ScriptableObject {
    // Octaves control the detailing
    // Persistance creates more hills
    // Redistribution creates more islands
    // Exponent generates more elevated plains, (highlands)

    public int octaves; 
    public float persistance;
    public float redistributionModifier;
    public float exponent;
    public float noiseZoom;
    public Vector2Int offset;
    public Vector2Int worldOffset;
}
