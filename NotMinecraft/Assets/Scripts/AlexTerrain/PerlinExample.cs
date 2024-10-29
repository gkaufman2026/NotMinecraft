using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creates a texture and fills it with perlin noise
 */

public class PerlinExample : MonoBehaviour
{
    // Width and height of the texture
    public int textureWidth;
    public int textureHeight;

    // Origin of area in the plane
    public float xOrigin;
    public float yOrigin;

    // The scale is the number of cycles of the noise pattern that is repeated over width and height of texture
    public float scale = 1.0f;

    private Texture2D noiseTexture;
    private Color[] pixel;
    private Renderer render;

    void Start()
    {
        render = GetComponet<Renderer>();

        noiseTexture = new Texture2D(textureWidth, textureHeight);
        pixel = new Color[noiseTexture.width * noiseTexture.height];
        render.material.mainTexture = noiseTexture;

    }

    void CalcNoise()
    {
        // For each pixel in the texture
        for (float y = 0.0f; y < noiseTexture.height; y++)
        {
            for (float x = 0.0f; x < noiseTexture.width; x++)
            {
                float xCord = xOrigin + x / noiseTexture.width * scale;
                float yCord = yOrigin + y / noiseTexture.height * scale;
                float sample = Mathf.PerlinNoise(xCord, yCord);
                pixel[(int)y * noiseTexture.width + (int)x] = new Color(sample, sample, sample);
            }
        }

        // Copy pixel data to the texture and load to GPU
        noiseTexture.SetPixels(pixel);
        noiseTexture.Apply();
    }

    void Update()
    {
        CalcNoise();
    }
}
