using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAvoidance : Constraint
{
    private float mAvoidanceRadius = 1f;

    public override bool isViolated(GameObject character, Vector3Int startPos, Vector3Int goalPos)
    {
        bool zombieInPath = false; //Code way to detect if zombie is in the way

        return zombieInPath;
    }

    public override Vector3Int suggestNewGoal(GameObject character, Vector3Int startPos, Vector3Int goalPos)
    {
        throw new System.NotImplementedException();
    }
}
