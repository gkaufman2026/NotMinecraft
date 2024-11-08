using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AlexMapGenerator))]

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
