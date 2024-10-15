using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVillager : MonoBehaviour
{
    public enum VillagerTypes {
        NORMAL,
        ZOMBIE
    }
    public VillagerTypes type;

    void Update()
    {
        Material[] textures = TextureManager.Instance.villagerTextures;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers) {
            renderer.material = textures[(int)type];
        }
    }
}
