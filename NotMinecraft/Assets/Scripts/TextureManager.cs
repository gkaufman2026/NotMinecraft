using UnityEngine;

public class TextureManager : MonoBehaviour {
    public static TextureManager Instance { get; private set; }
    public Material[] villagerTextures;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
