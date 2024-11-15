using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Constraint
{
    public abstract bool isViolated(GameObject character, Vector3 startPos, Vector3Int goalPos);

    public abstract Vector3Int suggestNewGoal(GameObject character, Vector3 startPos, Vector3Int goalPos);
}
