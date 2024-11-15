using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposer
{
    private List<List<Vector3Int>> currentPaths = new();
    private float satisfactionRadiusSquared = 1.0f;

    public List<Vector3Int> getCurrPath()
    {
        if (currentPaths.Count > 0)
        {
            return currentPaths[0];
        }

        return null;
    }

    public Nullable<Vector3Int> getCurrSubGoal()
    {
        List<Vector3Int> currPath = getCurrPath();
        if (currPath.Count > 0)
        {
            return currPath[0];
        }

        return null;
    }

    public Nullable<Vector3Int> getCurrSubGoal(Vector3 characterPos)
    {
        List<Vector3Int> currPath = getCurrPath();
        if (currPath.Count > 0)
        {
            Vector3Int currSubGoal = currPath[0];
            while (currPath.Count > 0)
            {
                float squaredDist = (characterPos - currSubGoal).sqrMagnitude;

                if (squaredDist <= satisfactionRadiusSquared)
                {
                    achievedCurrSubGoal();
                } 
                else
                {
                    break;
                }
            }

            if (currPath.Count > 0)
            {
                return currSubGoal;
            }

            return null;
        }

        return null;
    }

    public void achievedCurrSubGoal()
    {
        Nullable<Vector3Int> currSubGoal = getCurrSubGoal();
        if (currSubGoal != null)
        {
            currentPaths[0].Remove((Vector3Int)currSubGoal);

            if (currentPaths[0].Count == 0)
            {
                currentPaths.RemoveAt(0);
            }
        }
    }

    public void updateCurrPathGoals(Vector3 characterPos, Vector3Int subGoal)
    {
        float squaredDist = (characterPos - subGoal).sqrMagnitude;

        if (squaredDist <= satisfactionRadiusSquared)
        {
            achievedCurrSubGoal();
        }
    }

    public void buildAndAddNextPath(ref World world, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> newPath = PathMaker.generatePath(ref world, start, goal);
        currentPaths.Add(newPath);
    }
}
