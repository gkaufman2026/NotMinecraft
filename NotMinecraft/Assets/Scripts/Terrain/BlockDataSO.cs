using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Data", menuName = "NotMinecraft/Block Data")]
public class BlockDataSO : ScriptableObject {
    public Vector2 textureSize;
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData {
    public BlockType blockType;
    public Vector2Int up, side, down;
    public bool isSolid = true, hasCollider = true;
}
