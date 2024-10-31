using UnityEngine;
using ImGuiNET;

public class TerrainUI : MonoBehaviour {
    private World world;
    private bool showsControls, showsSettings;

    private void Awake() {
        world = FindAnyObjectByType<World>();
    }

    void OnEnable() {
        ImGuiUn.Layout += OnLayout;
    }

    void OnDisable() {
        ImGuiUn.Layout -= OnLayout;
    }

    void OnLayout() {
        ShowHeaderBar();
    }

    private void ShowHeaderBar() {
        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Terrain Generation")) {
            ImGui.MenuItem("Controls", null, ref showsControls);
            ImGui.Separator();
            ImGui.MenuItem("Terrain Settings", null, ref showsSettings);
            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();
    }
}