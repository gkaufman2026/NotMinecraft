using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposer
{
    private List<Vector3Int> mCurrentPath = new();
    private float satisfactionXSquared = 0.5f;
    private float satisfactionYSquared = 1f;

    public List<Vector3Int> getCurrPath()
    {
        return mCurrentPath;
    }

    public Nullable<Vector3Int> getCurrSubGoal()
    {
        if (mCurrentPath.Count > 0)
        {
            return mCurrentPath[mCurrentPath.Count - 1];
        }

        return null;
    }

    public Nullable<Vector3Int> getCurrSubGoal(Vector3 characterPos)
    {
        if (mCurrentPath.Count > 0)
        {
            Vector3Int currSubGoal = Vector3Int.zero;
            while (mCurrentPath.Count > 0)
            {
                currSubGoal = mCurrentPath[mCurrentPath.Count - 1];
                Vector3 dist = characterPos - currSubGoal;
                float squaredDistX = dist.x * dist.x;
                float squaredDistY = dist.y * dist.y;

                if ((squaredDistX <= satisfactionXSquared) && (squaredDistY <= satisfactionYSquared))
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
        Vector3 dist = characterPos - subGoal;
        float squaredDistX = dist.x * dist.x;
        float squaredDistY = dist.y * dist.y;

        if ((squaredDistX <= satisfactionXSquared) && (squaredDistY <= satisfactionYSquared))
        {
            achievedCurrSubGoal();
        }
    }

    public void buildAndAddNextPath(ref World world, Vector3Int start, Vector3Int goal)
    {
        mCurrentPath = PathMaker.generatePath(ref world, start, goal);
    }
}
