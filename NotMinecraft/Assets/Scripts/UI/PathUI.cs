using ImGuiNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUI : TerrainUI
{
    [SerializeField] private GameObject _mMobPrefab;
    private Vector3 mStartPos = new Vector3(15, 9, 7);
    private Vector3 mGoalPos = new Vector3(28, 10, 19);

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
        ImGui.Text("Goal X");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##GoalXSlider", ref mGoalPos.x, -1000f, 1000f);

        ImGui.Text("Goal Y");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##GoalYSlider", ref mGoalPos.y, -1000f, 1000f);

        ImGui.Text("Goal Z");
        ImGui.SameLine(labelWidth);
        ImGui.SetNextItemWidth(width);
        ImGui.SliderFloat("##GoalZSlider", ref mGoalPos.z, -1000f, 1000f);

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
            runPath();
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
        GameObject mob = Instantiate(_mMobPrefab, mStartPos, Quaternion.identity);
        Mob mobC = mob.GetComponent<Mob>();

        if (mobC != null)
        {
            mobC.setGoal(new Vector3Int((int)mGoalPos.x, (int)mGoalPos.y, (int)mGoalPos.z), ref world);
        }
    }
}
