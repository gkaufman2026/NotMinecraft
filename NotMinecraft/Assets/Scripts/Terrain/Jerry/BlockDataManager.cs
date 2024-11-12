using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour {
    public static Vector2 tileSize;
    public static Dictionary<BlockType, TextureData> textureList = new Dictionary<BlockType, TextureData>();
    public BlockDataSO textureData;
    public static float textureOffset = 0.001f;

    private void Awake() {
        foreach (var item in textureData.textureDataList) {
            if (!textureList.ContainsKey(item.blockType)) {
                textureList.Add(item.blockType, item);
            }
        }
        tileSize = textureData.textureSize;
    }
}