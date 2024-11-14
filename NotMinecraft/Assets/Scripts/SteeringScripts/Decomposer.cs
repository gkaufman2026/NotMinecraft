using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposer
{
    //Add range of satasfaction to be true
    private List<Vector3Int> currentPath;

    public Nullable<Vector3Int> getCurrSubGoal()
    {
        if (currentPath.Count > 0)
        {
            return currentPath[0];
        }

        return null;
    }

    public void achievedCurrSubGoal()
    {
        Nullable<Vector3Int> currSubGoal = getCurrSubGoal();
        if (currSubGoal != null)
        {
            currentPath.Remove((Vector3Int)currSubGoal);
        }
    }

    private List<Vector3Int> buildNextPath(ref World world, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> newPath = PathMaker.generatePath(ref world, start, goal);

        currentPath = newPath;

        return newPath;
    }
}
