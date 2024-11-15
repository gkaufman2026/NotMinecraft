using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator
{
    public struct Action
    {
        public Vector3 walkingVelocity;
        public float jumpVelocity;
        public bool openDoor;

        public Action(Vector3 walkingVelocity, float jumpVelocity, bool openDoor)
        {
            this.walkingVelocity = walkingVelocity;
            this.jumpVelocity = jumpVelocity;
            this.openDoor = openDoor;
        }
    }

    public Action getActionToPerform(GameObject character, Vector3 start, Vector3 goal)
    {
        Vector3 dirVec = goal - start;
        float yDiffSquared = dirVec.y * dirVec.y;

        Action currAction = new Action();

        if (yDiffSquared == 0)
        {
            currAction.jumpVelocity = 1f;
        }

        currAction.walkingVelocity = dirVec.normalized;

        //Add way to find a door here to mark if the character should open the door
        currAction.openDoor = false;

        return currAction;
    }
}
