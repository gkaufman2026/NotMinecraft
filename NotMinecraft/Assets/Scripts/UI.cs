using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    private World world;
    private void Awake()
    {
        world = FindAnyObjectByType<World>();
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 180), "Terrain Menu");
        if (GUI.Button(new Rect(20, 40, 80, 20), "Generate"))  {
            world.GenerateWorld();
        }
        if (GUI.Button(new Rect(20, 65, 80, 20), "Clear"))   {
            world.ClearWorld();
        }

        world.mapSizeInChunks = (int) GUI.HorizontalSlider(new Rect(20, 90, 80, 30), world.mapSizeInChunks, 1.0f, 10.0f);
        world.chunkSize = (int) GUI.HorizontalSlider(new Rect(20, 110, 80, 30), world.chunkSize, 1.0f, 20.0f);
        world.chunkHeight = (int)GUI.HorizontalSlider(new Rect(20, 130, 80, 30), world.chunkHeight, 50.0f, 200.0f);
        world.waterThreshold = (int)GUI.HorizontalSlider(new Rect(20, 150, 80, 30), world.waterThreshold, 1.0f, 75.0f);
        world.noiseScale = GUI.HorizontalSlider(new Rect(20, 170, 80, 30), world.noiseScale, 0.0f, 1.0f);
    }
}
