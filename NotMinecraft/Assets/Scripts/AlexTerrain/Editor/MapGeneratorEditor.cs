using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AlexMapGenerator))]

// Makes it so whenever a value for the map generator is updated in the editor,
// the map automatically regenerates if auto update is on
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AlexMapGenerator mapGen = (AlexMapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.drawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.drawMapInEditor();
        }
    }
}
