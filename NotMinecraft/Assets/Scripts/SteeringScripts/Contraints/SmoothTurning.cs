using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurning : Constraint
{
    private float maxTurnAngle = 20f;

    public override bool isViolated(GameObject character, Vector3Int startPos, Vector3Int goalPos)
    {
        float currTurnAngle = (Vector3.Dot((goalPos - startPos), character.transform.forward) - 1) * 0.5f * -180;
        if (currTurnAngle > maxTurnAngle)
        {
            return false;
        }

        return true;
    }

    public override Vector3 suggestNewGoal(GameObject character, Vector3Int startPos, Vector3Int goalPos)
    {
        throw new System.NotImplementedException();
    }
}
