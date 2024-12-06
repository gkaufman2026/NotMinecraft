using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator
{
    public struct Action
    {
        public Vector2 walkingVelocity;
        public float jumpVelocity;
        public bool openDoor;
        public bool idle;

        public Action(Vector3 walkingVelocity, float jumpVelocity, bool openDoor, bool isIdle)
        {
            this.walkingVelocity = walkingVelocity;
            this.jumpVelocity = jumpVelocity;
            this.openDoor = openDoor;
            this.idle = isIdle;
        }
    }

    public Action getActionToPerform(GameObject character, Vector3 start, Vector3 goal, Vector3 additionalForces)
    {
        Vector3 dirVec = goal - start;
        float yDiffSquared = dirVec.y * dirVec.y;

        Action currAction = new Action();

        if (yDiffSquared >= 1f && dirVec.y > 0)
        {
            currAction.jumpVelocity = 1f;
        }

        currAction.walkingVelocity = new Vector2(dirVec.x, dirVec.z).normalized;

        Mob mobData = character.GetComponent<Mob>();
        if (mobData != null)
        {
            currAction.walkingVelocity *= mobData.WalkSpeed;
            currAction.walkingVelocity += new Vector2(additionalForces.x, additionalForces.z);
            currAction.walkingVelocity = currAction.walkingVelocity.normalized;
            currAction.walkingVelocity *= mobData.WalkSpeed;

            currAction.jumpVelocity *= mobData.MaxJumpPower;
        }

        //Add way to find a door here to mark if the character should open the door
        currAction.openDoor = false;
        currAction.idle = false;

        return currAction;
    }
}
