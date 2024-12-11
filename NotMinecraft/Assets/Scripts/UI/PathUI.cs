using ImGuiNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUI : TerrainUI
{
    [SerializeField] private GameObject _mVillagerPrefab;
    [SerializeField] private GameObject _mZombiePrefab;
    private Vector3 mStartPos = new Vector3(85, 14, 16);
    private Vector3 mStartPos2 = new Vector3(87, 14, 18);
    private Vector3 mZombieStartPos = new Vector3(89, 15, 16);

    private void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
    }

    private void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

    protected override void renderImGuiThings()
    {
        ImGui.Text("Start X");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##StartXSlider", ref mStartPos.x, -1000f, 1000f);

        ImGui.Text("Start Y");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##StartYSlider", ref mStartPos.y, -1000f, 1000f);

        ImGui.Text("Start Z");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##StartZSlider", ref mStartPos.z, -1000f, 1000f);

        if (ImGui.Button("Make Path", new Vector2(width, 20)))
        {
            //runPath();
            GameObject villager = Instantiate(_mVillagerPrefab, mStartPos, Quaternion.identity);
            GameObject villager2 = Instantiate(_mVillagerPrefab, mStartPos2, Quaternion.identity);
            GameObject zombie = Instantiate(_mZombiePrefab, mZombieStartPos, Quaternion.identity);
        }
    }

    private void OnLayout()
    {
        if (!ImGui.Begin("Not Minecraft"))
        {
            ImGui.End();
            return;
        }

        if (ImGui.CollapsingHeader("World Settings"))
        {
            base.renderImGuiThings();
        }

        if (ImGui.CollapsingHeader("Path Settings"))
        {
            renderImGuiThings();
        }

        ImGui.End();
    }

    private void runPath()
    {
        GameObject villager = Instantiate(_mVillagerPrefab, mStartPos, Quaternion.identity);
        Villager villagerScript = villager.GetComponent<Villager>();

        if (villagerScript != null)
        {
            //villagerScript.setGoal(new Vector3Int((int)mGoalPos.x, (int)mGoalPos.y, (int)mGoalPos.z), world);
        }
    }
}
