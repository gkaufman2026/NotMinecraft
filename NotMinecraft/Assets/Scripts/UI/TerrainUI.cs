using UnityEngine;
using ImGuiNET;


public class TerrainUI : MonoBehaviour {
    private World world;
    private CameraTest movement;

    // Default values
    private int defaultMapSize, defaultChunkSize, defaultChunkHeight, defaultWaterThreshold;
    private float defaultNoiseScale;
    private Vector2 defaultOffset;

    private void Awake() {
        world = FindAnyObjectByType<World>();
        movement = FindAnyObjectByType<CameraTest>();

        defaultMapSize = world.mapSizeInChunks;
        defaultChunkSize = world.chunkSize;
        defaultChunkHeight = world.chunkHeight;
        defaultWaterThreshold = world.waterThreshold;
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
        // Settings for Procedural Generation
        float labelWidth = 130.0f, checkboxWidth = 150f, width = 100f; 

        ImGui.Text("Size In Chunks");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##SizeInChunksSlider", ref world.mapSizeInChunks, 1, 20);

        ImGui.Text("Chunk Size");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkSizeSlider", ref world.chunkSize, 1, 20);

        ImGui.Text("Chunk Height");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkHeightSlider", ref world.chunkHeight, 50, 200);

        ImGui.Text("Water Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##WaterThresholdSlider", ref world.waterThreshold, 1, 50);

        ImGui.Text("Noise Scale");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##NoiseScaleSlider", ref world.noiseScale, 0, 0.05f);

        ImGui.NewLine();

        // Offset Options
        ImGui.Text("X Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##XOffsetSlider", ref world.offset.x, -300f, 300f);

        ImGui.Text("Y Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##YOffsetSlider", ref world.offset.y, -300f, 300f);

        ImGui.Text("Z Offset");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##ZOffsetSlider", ref world.offset.z, -300f, 300f);

        ImGui.NewLine();

        if (ImGui.CollapsingHeader("Player Settings")) {
            ImGui.Text("Sprint Movement");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderFloat("##SprintMovementSlider", ref movement.fastMovementSpeed, 50f, 200f);

            ImGui.Text("Sensitivity");
            ImGui.SameLine(labelWidth);
            ImGui.SetNextItemWidth(width);
            ImGui.SliderFloat("##SensitivitySlider", ref movement.sensitivity, 0f, 10f);
        }

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

        ImGui.Separator();

        // Centering Buttons
        float spacing = ImGui.GetStyle().ItemSpacing.x;
        float totalWidth = width * 2 + spacing;
        float windowWidth = ImGui.GetWindowWidth();
        float cursorPosX = (windowWidth - totalWidth) * 0.5f;
        ImGui.SetCursorPosX(cursorPosX);

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
            world.noiseScale = defaultNoiseScale;
            world.offset = defaultOffset;
        }

        ImGui.End();
    }
}