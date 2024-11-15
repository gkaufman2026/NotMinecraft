using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposer
{
    private List<Vector3Int> mCurrentPath = new();
    private float satisfactionRadiusSquared = 0.5f;

    public List<Vector3Int> getCurrPath()
    {
        return mCurrentPath;
    }

    public Nullable<Vector3Int> getCurrSubGoal()
    {
        if (mCurrentPath.Count > 0)
        {
            return mCurrentPath[0];
        }

        return null;
    }

    public Nullable<Vector3Int> getCurrSubGoal(Vector3 characterPos)
    {
        if (mCurrentPath.Count > 0)
        {
            Vector3Int currSubGoal = mCurrentPath[0];
            while (mCurrentPath.Count > 0)
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

            if (mCurrentPath.Count > 0)
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
            mCurrentPath.Remove((Vector3Int)currSubGoal);
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
        mCurrentPath = PathMaker.generatePath(ref world, start, goal);
    }
}
