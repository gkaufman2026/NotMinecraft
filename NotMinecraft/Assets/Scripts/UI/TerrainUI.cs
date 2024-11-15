using UnityEngine;
using ImGuiNET;

public class TerrainUI : MonoBehaviour {
    private World world;
    private BiomeGenerator biomeGenerator;
    private CameraTest movement;

    // Default values
    private int defaultMapSize, defaultChunkSize, defaultChunkHeight, defaultWaterThreshold, defaultStoneTreshold, defaultSandThreshold;
    private float defaultNoiseScale;
    private Vector3 defaultOffset;

    // Min Values
    private const int MIN_MAP_SIZE = 1, MIN_CHUNK_SIZE = 1, MIN_CHUNK_HEIGHT = 50;
    private const int MIN_WATER_THRESHOLD = 1, MIN_STONE_THRESHOLD = 1, MIN_SAND_THRESHOLD = 1;
    private const float MIN_NOISE_SCALE = 0f;

    // Max Values
    private const int MAX_MAP_SIZE = 20, MAX_CHUNK_SIZE = 20, MAX_CHUNK_HEIGHT = 200;
    private const int MAX_WATER_THRESHOLD = 50, MAX_STONE_THRESHOLD = 75, MAX_SAND_THRESHOLD = 10;
    private const float MAX_NOISE_SCALE = 0.05f;

    // Settings for Procedural Generation
    private const float labelWidth = 130.0f, checkboxWidth = 150f, width = 100f, presetWidth = 50f;

    // UI Variables
    private float spacing, totalWidth, windowWidth, cursorPosX;

    private void Awake() {
        world = FindAnyObjectByType<World>();
        biomeGenerator = FindAnyObjectByType<BiomeGenerator>();
        movement = FindAnyObjectByType<CameraTest>();

        // On Start of Game, the default settings are stored for reset button
        defaultMapSize = world.mapSizeInChunks;
        defaultChunkSize = world.chunkSize;
        defaultChunkHeight = world.chunkHeight;
        defaultWaterThreshold = biomeGenerator.waterThreshold;
        defaultStoneTreshold = biomeGenerator.stoneThreshold;
        defaultSandThreshold = biomeGenerator.sandThreshold;
        defaultNoiseScale = biomeGenerator.noiseScale;
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
            biomeGenerator.waterThreshold = defaultWaterThreshold;
            biomeGenerator.sandThreshold = defaultSandThreshold;
            biomeGenerator.stoneThreshold = defaultStoneTreshold;
            biomeGenerator.noiseScale = defaultNoiseScale;
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
                biomeGenerator.waterThreshold = 39;
                biomeGenerator.stoneThreshold = 60;
                biomeGenerator.sandThreshold = 4;
                biomeGenerator.noiseScale = 0.01f;
                world.offset = new Vector3(-120, -45, -145);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#2", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 20;
                world.chunkSize = 20;
                world.chunkHeight = 200;
                biomeGenerator.waterThreshold = 50;
                biomeGenerator.stoneThreshold = 60;
                biomeGenerator.sandThreshold = 6;
                biomeGenerator.noiseScale = 0.05f;
                world.offset = new Vector3(-565, -185, -485);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#3", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 6;
                world.chunkSize = 16;
                world.chunkHeight = 100;
                biomeGenerator.waterThreshold = 31;
                biomeGenerator.stoneThreshold = 60;
                biomeGenerator.sandThreshold = 4;
                biomeGenerator.noiseScale = 0.01f;
                world.offset = new Vector3(-145, -45, -145);
                world.GenerateWorld();
            }
            ImGui.SameLine();
            if (ImGui.Button("#4", new Vector2(presetWidth, 20))) {
                world.ClearWorld();
                world.mapSizeInChunks = 12;
                world.chunkSize = 12;
                world.chunkHeight = 150;
                biomeGenerator.waterThreshold = 50;
                biomeGenerator.stoneThreshold = 20;
                biomeGenerator.sandThreshold = 4;
                biomeGenerator.noiseScale = 0.024f;
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
        ImGui.SliderInt("##SizeInChunksSlider", ref world.mapSizeInChunks, MIN_MAP_SIZE, MAX_MAP_SIZE);

        ImGui.Text("Chunk Size");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkSizeSlider", ref world.chunkSize, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);

        ImGui.Text("Chunk Height");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##ChunkHeightSlider", ref world.chunkHeight, MIN_CHUNK_HEIGHT, MAX_CHUNK_HEIGHT);

        ImGui.Text("Water Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##WaterThresholdSlider", ref biomeGenerator.waterThreshold, MIN_WATER_THRESHOLD, MAX_WATER_THRESHOLD);

        ImGui.Text("Stone Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##StoneThresholdSlider", ref biomeGenerator.stoneThreshold, MIN_STONE_THRESHOLD, MAX_STONE_THRESHOLD);

        ImGui.Text("Sand Threshold");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderInt("##SandThresholdSlider", ref biomeGenerator.sandThreshold, MIN_SAND_THRESHOLD, MAX_SAND_THRESHOLD);

        ImGui.Text("Noise Scale");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##NoiseScaleSlider", ref biomeGenerator.noiseScale, MIN_NOISE_SCALE, MAX_NOISE_SCALE);

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

    // Using Min and Max values set and generating randoms and assigning to the var
    private void GenerateRandomWorld() {
        world.mapSizeInChunks = UnityEngine.Random.Range(MIN_MAP_SIZE, MAX_MAP_SIZE + 1);
        world.chunkSize = UnityEngine.Random.Range(MIN_CHUNK_SIZE, MAX_CHUNK_SIZE + 1);
        world.chunkHeight = UnityEngine.Random.Range(MIN_CHUNK_HEIGHT, MAX_CHUNK_HEIGHT + 1);
        biomeGenerator.waterThreshold = UnityEngine.Random.Range(MIN_WATER_THRESHOLD, MAX_WATER_THRESHOLD + 1);
        biomeGenerator.stoneThreshold = UnityEngine.Random.Range(MIN_STONE_THRESHOLD, MAX_STONE_THRESHOLD + 1);
        biomeGenerator.sandThreshold = UnityEngine.Random.Range(MIN_SAND_THRESHOLD, MAX_SAND_THRESHOLD + 1);
        biomeGenerator.noiseScale = UnityEngine.Random.Range(MIN_NOISE_SCALE, MAX_NOISE_SCALE);
    }

    private void GenerateCenteredLayout() {
        spacing = ImGui.GetStyle().ItemSpacing.x;
        totalWidth = width * 2 + spacing;
        windowWidth = ImGui.GetWindowWidth();
        cursorPosX = (windowWidth - totalWidth) * 0.5f;
        ImGui.SetCursorPosX(cursorPosX);
    }
}
