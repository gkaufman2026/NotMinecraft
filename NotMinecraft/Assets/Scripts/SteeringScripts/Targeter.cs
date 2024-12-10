using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter
{
    private List<Vector3Int> targets = new();

    public void addGoal(Vector3Int goal)
    {
        targets.Add(goal);
    }

    public Nullable<Vector3Int> getCurrentTarget()
    {
        if (targets.Count > 0)
        {
            return targets[0];
        }

        return null;
    }

    public void achievedCurrTarget()
    {
        Nullable<Vector3Int> currTarget = getCurrentTarget();
        if (currTarget != null)
        {
            targets.Remove((Vector3Int)currTarget);
        }
    }

    public void clearGoals()
    {
        targets.Clear();
    }
}
