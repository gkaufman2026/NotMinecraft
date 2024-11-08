using UnityEngine;

// Outdated Script, Was Used Before Dear ImGui Implementation
public class UI : MonoBehaviour {
    private World world;
    private bool showMenu = true;

    private void Awake() {
        world = FindAnyObjectByType<World>();
    }

    void OnGUI() {
        GUIStyle boxStyle = new(GUI.skin.box) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperCenter };
        GUIStyle buttonStyle = new(GUI.skin.button) { fontSize = 12, fontStyle = FontStyle.Normal };
        GUIStyle sliderLabelStyle = new(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Normal, padding = new RectOffset(5, 5, 0, 0) };

        showMenu = GUI.Toggle(new Rect(10, 10, 25, 25), showMenu, "=", buttonStyle);

        if (showMenu) {
            GUI.Box(new Rect(10, 40, 200, 205), "", boxStyle);

            if (GUI.Button(new Rect(20, 50, 75, 25), "Generate", buttonStyle)) {
                world.GenerateWorld();
            }
            if (GUI.Button(new Rect(125, 50, 75, 25), "Clear", buttonStyle)) {
                world.ClearWorld();
            }

            GUI.Label(new Rect(20, 85, 180, 20), "Map Size in Chunks:", sliderLabelStyle);
            world.mapSizeInChunks = (int)GUI.HorizontalSlider(new Rect(20, 105, 180, 20), world.mapSizeInChunks, 1.0f, 10.0f);

            GUI.Label(new Rect(20, 120, 180, 20), "Chunk Size:", sliderLabelStyle);
            world.chunkSize = (int)GUI.HorizontalSlider(new Rect(20, 135, 180, 20), world.chunkSize, 1.0f, 20.0f);

            GUI.Label(new Rect(20, 150, 180, 20), "Chunk Height:", sliderLabelStyle);
            world.chunkHeight = (int)GUI.HorizontalSlider(new Rect(20, 165, 180, 20), world.chunkHeight, 50.0f, 200.0f);

            GUI.Label(new Rect(20, 180, 180, 20), "Water Threshold:", sliderLabelStyle);
            world.waterThreshold = (int)GUI.HorizontalSlider(new Rect(20, 195, 180, 20), world.waterThreshold, 1.0f, 75.0f);

            GUI.Label(new Rect(20, 210, 180, 20), "Noise Scale:", sliderLabelStyle);
            world.noiseScale = GUI.HorizontalSlider(new Rect(20, 225, 180, 20), world.noiseScale, 0.0f, 1.0f);
        }
    }
}
