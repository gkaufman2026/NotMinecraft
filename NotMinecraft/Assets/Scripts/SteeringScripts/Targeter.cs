using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter
{
    List<Vector3Int> targets = new();

    public Vector3Int getNextTarget()
    {
        return targets[0];
    }
}
