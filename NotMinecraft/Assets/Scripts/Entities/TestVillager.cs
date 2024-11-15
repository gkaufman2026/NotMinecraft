using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class TestVillager : MonoBehaviour
{
    public enum VillagerTypes {
        NORMAL,
        ZOMBIE
    }
    public VillagerTypes type;
    private Villager villager;

    private void Awake() {
        villager = GetComponent<Villager>();
    }

    void Update()
    {
        Material[] textures = TextureManager.Instance.villagerTextures;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers) {
            renderer.material = textures[(int)type];
        }

        if (type == VillagerTypes.ZOMBIE) {
        }
    }
}
