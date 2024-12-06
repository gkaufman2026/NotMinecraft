using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposer
{
    private List<Vector3Int> mCurrentPath = new();
    private float satisfactionXSquaredDest = 0.5f;
    private float satisfactionZSquaredDest = 0.5f;
    private float satisfactionYSquaredDest = 1f;

    private float satisfactionXSquared = 25f;
    private float satisfactionZSquared = 25f;
    private float satisfactionYSquared = 25f;

    public List<Vector3Int> getCurrPath()
    {
        return mCurrentPath;
    }

    public Nullable<Vector3Int> getCurrSubGoal()
    {
        if (mCurrentPath.Count > 0)
        {
            Vector3Int currSubGoal = mCurrentPath[mCurrentPath.Count - 1];
            Vector3Int nextSubGoal;
            return currSubGoal;
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
                float squaredDistZ = dist.z * dist.z;

                if (mCurrentPath.Count - 1 != 0)
                {
                    if ((squaredDistX <= satisfactionXSquared) && (squaredDistY <= satisfactionYSquared) && (squaredDistZ <= satisfactionZSquared))
                    {
                        achievedCurrSubGoal();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if ((squaredDistX <= satisfactionXSquaredDest) && (squaredDistY <= satisfactionYSquaredDest) && (squaredDistZ <= satisfactionZSquaredDest))
                    {
                        achievedCurrSubGoal();
                    }
                    else
                    {
                        break;
                    }
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

    public void setCurrSubGoal(Vector3Int newSubGoal)
    {
        if (mCurrentPath.Count > 0)
        {
            mCurrentPath[mCurrentPath.Count - 1] = newSubGoal;
        }
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
        float squaredDistZ = dist.z * dist.z;

        if (mCurrentPath.Count - 1 != 0)
        {
            if ((squaredDistX <= satisfactionXSquared) && (squaredDistY <= satisfactionYSquared) && (squaredDistZ <= satisfactionZSquared))
            {
                achievedCurrSubGoal();
            }
        }
        else
        {
            if ((squaredDistX <= satisfactionXSquaredDest) && (squaredDistY <= satisfactionYSquaredDest) && (squaredDistZ <= satisfactionZSquaredDest))
            {
                achievedCurrSubGoal();
            }
        }
    }

    public void buildAndAddNextPath(ref World world, Vector3Int start, Vector3Int goal)
    {
        mCurrentPath = PathMaker.generatePath(ref world, start, goal);
    }
}
