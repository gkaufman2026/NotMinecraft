using ImGuiNET;
using UnityEngine;
using System;

public class ImGui : MonoBehaviour
{
    AlexMapGenerator mapGenerator;

    private bool autoUpdateToggle = true;
    private float persistance = 0.5f;

    private void Awake()
    {
        /*
        GUI.BeginGroup(new Rect(25, 25, 200, 500));

        GUI.Box(new Rect(0, 0, 200, 500), "Mesh Generation Options");

        GUI.Label(new Rect(10, 60, 120, 30), "Persistance: " + Mathf.Round(persistance * 100) / 100.0);
        persistance = GUI.HorizontalSlider(new Rect(10, 80, 100, 30), persistance, 0.0f, 1.0f);

        autoUpdateToggle = GUI.Toggle(new Rect(10, 380, 90, 30), autoUpdateToggle, "Auto Update");

        GUI.Button(new Rect(5, 400, 190, 20), "Generate Mesh");

        GUI.EndGroup();
        */

        void Awake()
        {
            
        }

        void OnLayout()
        {

        }

        void OnEnable()
        {
            ImGuiUn.Layout += OnLayout;
        }

        void OnDisable()
        {
            ImGuiUn.Layout -= OnLayout;
        }
    }
}
