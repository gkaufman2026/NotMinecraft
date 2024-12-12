using UnityEngine;
using ImGuiNET;
using System;

public class TerrainUI : MonoBehaviour {
    protected World world;
    protected CameraTest movement;

    // Default values
    protected int defaultMapSize, defaultChunkSize, defaultChunkHeight;

    // Min Values
    protected const int MIN_MAP_SIZE = 1, MIN_CHUNK_SIZE = 1, MIN_CHUNK_HEIGHT = 50;

    // Max Values
    protected const int MAX_MAP_SIZE = 20, MAX_CHUNK_SIZE = 20, MAX_CHUNK_HEIGHT = 200;

    // Settings for Procedural Generation
    protected const float labelWidth = 130.0f, checkboxWidth = 150f, width = 100f, presetWidth = 50f;

    // UI Variables
    protected float spacing, totalWidth, windowWidth, cursorPosX;

    private void Awake() {
        world = FindAnyObjectByType<World>();
        movement = FindAnyObjectByType<CameraTest>();

        // On Start of Game, the default settings are stored for reset button
        defaultMapSize = world.mapSizeInChunks;
        defaultChunkSize = world.chunkSize;
        defaultChunkHeight = world.chunkHeight;
    }

    private void OnEnable() {
        ImGuiUn.Layout += OnLayout;
    }

    private void OnDisable() {
        ImGuiUn.Layout -= OnLayout;
    }

    protected virtual void renderImGuiThings()
    {
        // Adding Sliders for Custom Generation Options
        AddSliders();

        // Adding Chunk Visuals for Scene Viewing
        AddChunkVisuals();

        // Adds entity visuals
        AddEntityVisuals();

        ImGui.Separator();

        // Centering Buttons
        GenerateCenteredLayout();

        // Buttons to Generate & Clear Terrain
        if (ImGui.Button("Generate", new Vector2(width, 20))) { world.GenerateWorld(); }
        ImGui.SameLine();
        if (ImGui.Button("Clear", new Vector2(width, 20))) { world.ClearWorld(); }

        ImGui.SetCursorPosX(cursorPosX + width / 2);
        if (ImGui.Button("Reset", new Vector2(width, 20)))
        {
            world.mapSizeInChunks = defaultMapSize;
            world.chunkSize = defaultChunkSize;
            world.chunkHeight = defaultChunkHeight;
        }
    }

    private void OnLayout() {
        if (!ImGui.Begin("Terrain Generation")) {
            ImGui.End();
            return;
        }

       renderImGuiThings();

        ImGui.End();
    }

    private void AddChunkVisuals() {
        if (ImGui.CollapsingHeader("Chunk Visuals")) {
            if (world.ChunkDictionary != null) {
                int chunkIndex = 1;
                // Creating buttons to visualize per chunk. 2 checkboxes per row
                for (int i = 0; i < world.ChunksParent.transform.childCount; i += 2) {
                    Transform chunkTransform = world.ChunksParent.transform.GetChild(i);
                    GameObject chunk = chunkTransform.gameObject;

                    if (chunk.TryGetComponent<ChunkRenderer>(out var chunkRenderer)) {
                        ImGui.SetNextItemWidth(checkboxWidth);
                        ImGui.Checkbox("Show Chunk " + chunkIndex++, ref chunkRenderer.showGizmo);
                    }

                    if (i + 1 < world.ChunksParent.transform.childCount) {
                        Transform nextChunkTransform = world.ChunksParent.transform.GetChild(i + 1);
                        GameObject nextChunk = nextChunkTransform.gameObject;
                        if (nextChunk.TryGetComponent<ChunkRenderer>(out var nextChunkRenderer)) {
                            ImGui.SameLine();
                            ImGui.SetNextItemWidth(checkboxWidth);
                            ImGui.Checkbox("Show Chunk " + chunkIndex++, ref nextChunkRenderer.showGizmo);
                        }
                    }
                    ImGui.NewLine();
                }

                // Toggle Button to Enable/Disable All
                if (ImGui.Button("Toggle Chunks", new Vector2(-1, 20))) {
                    foreach (Transform chunkTransform in world.ChunksParent.transform) {
                        GameObject chunk = chunkTransform.gameObject;
                        if (chunk.TryGetComponent<ChunkRenderer>(out var chunkRenderer)) {
                            chunkRenderer.showGizmo = !chunkRenderer.showGizmo;
                        }
                    }
                }
            }
        }
    }

    private void AddEntityVisuals()
    {
        if (ImGui.CollapsingHeader("Mob Settings"))
        {
            ImGui.Checkbox("Can Spawn Entities ", ref world.canSpawnEntities);

            ImGui.Text("Number of Villagers");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderInt("## Num of Villagers", ref world.mNumberOfVillagersToSpawn, 0, 250);
            ImGui.Text("Number of Zombies");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderInt("## Num of Zombies", ref world.mNumberOfZombiesToSpawn, 0, 250);
        }
    }

    private void AddPlayerSettings() {
        if (ImGui.CollapsingHeader("Player Settings")) {
            ImGui.Text("Sprint Movement");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderFloat("##SprintMovementSlider", ref movement.fastMovementSpeed, 50f, 250f);

            ImGui.Text("Sensitivity");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderFloat("##SensitivitySlider", ref movement.sensitivity, 0f, 10f);
        }
    }

    private void AddSliders() {
        ImGui.Text("Size In Chunks");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##SizeInChunksSlider", ref world.mapSizeInChunks, MIN_MAP_SIZE, MAX_MAP_SIZE);

        ImGui.Text("Chunk Size");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkSizeSlider", ref world.chunkSize, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);

        ImGui.Text("Chunk Height");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkHeightSlider", ref world.chunkHeight, MIN_CHUNK_HEIGHT, MAX_CHUNK_HEIGHT);
    }

    private void GenerateCenteredLayout() {
        spacing = ImGui.GetStyle().ItemSpacing.x;
        totalWidth = width * 2 + spacing;
        windowWidth = ImGui.GetWindowWidth();
        cursorPosX = (windowWidth - totalWidth) * 0.5f;
        ImGui.SetCursorPosX(cursorPosX);
    }
}
