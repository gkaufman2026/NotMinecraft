using UnityEngine;
using ImGuiNET;
using System;

public class TerrainUI : MonoBehaviour {
    private World world;
    private CameraTest movement;

    // Default values
    private int defaultMapSize, defaultChunkSize, defaultChunkHeight, defaultWaterThreshold, defaultStoneTreshold, defaultSandThreshold;
    private float defaultNoiseScale;
    private Vector3 defaultOffset;

    // Min Values
    private const int minMapSize = 1, minChunkSize = 1, minChunkHeight = 50;
    private const int minWaterThreshold = 1, minStoneThreshold = 1, minSandThreshold = 1;
    private const float minNoiseScale = 0f;

    // Max Values
    private const int maxMapSize = 20, maxChunkSize = 20, maxChunkHeight = 200;
    private const int maxWaterThreshold = 50, maxStoneThreshold = 75, maxSandThreshold = 10;
    private const float maxNoiseScale = 0.05f;

    // Settings for Procedural Generation
    private const float labelWidth = 130.0f, checkboxWidth = 150f, width = 100f, presetWidth = 50f;

    // UI Variables
    private float spacing, totalWidth, windowWidth, cursorPosX;

    private void Awake() {
        world = FindAnyObjectByType<World>();
        movement = FindAnyObjectByType<CameraTest>();

        defaultMapSize = world.mapSizeInChunks;
        defaultChunkSize = world.chunkSize;
        defaultChunkHeight = world.chunkHeight;
        defaultWaterThreshold = world.waterThreshold;
        defaultStoneTreshold = world.stoneThreshold;
        defaultSandThreshold = world.sandThreshold;
        defaultNoiseScale = world.noiseScale;
        defaultOffset = world.offset;
    }

    void OnEnable() {
        ImGuiUn.Layout += OnLayout;
    }

    void OnDisable() {
        ImGuiUn.Layout -= OnLayout;
    }

    void OnLayout() {
        if (!ImGui.Begin("Terrain Generation")) {
            ImGui.End();
            return;
        }

        // Adding Sliders for Custom Generation Options
        AddSliders();

        // Adding Player Settings for Game Viewing
        AddPlayerSettings();

        // Adding Chunk Visuals for Scene Viewing
        AddChunkVisuals();

        // Adding Presets for World Generation
        AddPresets();

        ImGui.Separator();

        // Centering Buttons
        GenerateCenteredLayout();

        // Buttons to Generate & Clear Terrain
        if (ImGui.Button("Generate", new Vector2(width, 20))) {  world.GenerateWorld(); }
        ImGui.SameLine(); 
        if (ImGui.Button("Clear", new Vector2(width, 20))) { world.ClearWorld(); }

        ImGui.SetCursorPosX(cursorPosX + width / 2);
        if (ImGui.Button("Reset", new Vector2(width, 20))) {
            world.mapSizeInChunks = defaultMapSize;
            world.chunkSize = defaultChunkSize;
            world.chunkHeight = defaultChunkHeight;
            world.waterThreshold = defaultWaterThreshold;
            world.sandThreshold = defaultSandThreshold;
            world.stoneThreshold = defaultStoneTreshold;
            world.noiseScale = defaultNoiseScale;
            world.offset = defaultOffset;
        }

        ImGui.End();
    }

    private void AddPresets() {
        if (ImGui.CollapsingHeader("Presets")) {
            GenerateCenteredLayout();

            float buttonSpacing = ImGui.GetStyle().ItemSpacing.x;
            float totalButtonWidth = (presetWidth * 4) + (buttonSpacing * 3);
            float windowWidth = ImGui.GetWindowWidth();
            float centerOffset = (windowWidth - totalButtonWidth) * 0.5f;

            ImGui.SetCursorPosX(centerOffset);
            if (ImGui.Button("#1", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 6;
                world.chunkSize = 16;
                world.chunkHeight = 100;
                world.waterThreshold = 39;
                world.stoneThreshold = 60;
                world.sandThreshold = 4;
                world.noiseScale = 0.01f;
                world.offset = new Vector3(-120, -45, -145);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#2", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 20;
                world.chunkSize = 20;
                world.chunkHeight = 200;
                world.waterThreshold = 50;
                world.stoneThreshold = 60;
                world.sandThreshold = 6;
                world.noiseScale = 0.05f;
                world.offset = new Vector3(-565, -185, -485);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#3", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 6;
                world.chunkSize = 16;
                world.chunkHeight = 100;
                world.waterThreshold = 31;
                world.stoneThreshold = 60;
                world.sandThreshold = 4;
                world.noiseScale = 0.01f;
                world.offset = new Vector3(-145, -45, -145);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#4", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 12;
                world.chunkSize = 12;
                world.chunkHeight = 150;
                world.waterThreshold = 50;
                world.stoneThreshold = 20;
                world.sandThreshold = 4;
                world.noiseScale = 0.024f;
                world.offset = new Vector3(-175, -115, -145);
                world.GenerateWorld();
            }

            float randomButtonWidth = width;
            float randomButtonOffset = (windowWidth - randomButtonWidth) * 0.5f;
            ImGui.SetCursorPosX(randomButtonOffset);

            if (ImGui.Button("Random", new Vector2(randomButtonWidth, 20))) {
                world.ClearWorld();
                GenerateRandomWorld();
                world.GenerateWorld();
            }
        }
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
        ImGui.SliderInt("##SizeInChunksSlider", ref world.mapSizeInChunks, minMapSize, maxMapSize);

        ImGui.Text("Chunk Size");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkSizeSlider", ref world.chunkSize, minChunkSize, maxChunkSize);

        ImGui.Text("Chunk Height");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkHeightSlider", ref world.chunkHeight, minChunkHeight, maxChunkHeight);

        ImGui.Text("Water Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##WaterThresholdSlider", ref world.waterThreshold, minWaterThreshold, maxWaterThreshold);

        ImGui.Text("Stone Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##StoneThresholdSlider", ref world.stoneThreshold, minStoneThreshold, maxStoneThreshold);

        ImGui.Text("Sand Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##SandThresholdSlider", ref world.sandThreshold, minSandThreshold, maxSandThreshold);

        ImGui.Text("Noise Scale");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##NoiseScaleSlider", ref world.noiseScale, minNoiseScale, maxNoiseScale);

        // Offset Options
        ImGui.Text("X Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##XOffsetSlider", ref world.offset.x, -1000f, 1000f);

        ImGui.Text("Y Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##YOffsetSlider", ref world.offset.y, -1000f, 1000f);

        ImGui.Text("Z Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##ZOffsetSlider", ref world.offset.z, -1000f, 1000f);
    }

    private void GenerateRandomWorld() {
        world.mapSizeInChunks = UnityEngine.Random.Range(minMapSize, maxMapSize + 1);
        world.chunkSize = UnityEngine.Random.Range(minChunkSize, maxChunkSize + 1);
        world.chunkHeight = UnityEngine.Random.Range(minChunkHeight, maxChunkHeight + 1);
        world.waterThreshold = UnityEngine.Random.Range(minWaterThreshold, maxWaterThreshold + 1);
        world.stoneThreshold = UnityEngine.Random.Range(minStoneThreshold, maxStoneThreshold + 1);
        world.sandThreshold = UnityEngine.Random.Range(minSandThreshold, maxSandThreshold + 1);
        world.noiseScale = UnityEngine.Random.Range(minNoiseScale, maxNoiseScale);
    }

    private void GenerateCenteredLayout() {
        spacing = ImGui.GetStyle().ItemSpacing.x;
        totalWidth = width * 2 + spacing;
        windowWidth = ImGui.GetWindowWidth();
        cursorPosX = (windowWidth - totalWidth) * 0.5f;
        ImGui.SetCursorPosX(cursorPosX);
    }
}
