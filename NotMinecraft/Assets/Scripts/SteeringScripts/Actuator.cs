using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator : MonoBehaviour
{
    private List<Vector3Int> goals;

    public Nullable<Vector3Int> getCurrentGoal()
    {
        if (goals.Count > 0)
        {
            return goals[0];
        }

        return null;
    }

    public void achievedCurrGoal()
    {
        Nullable<Vector3Int> currGoal = getCurrentGoal();
        if (currGoal != null)
        {
            goals.Remove((Vector3Int)currGoal);
        }
    }
}
