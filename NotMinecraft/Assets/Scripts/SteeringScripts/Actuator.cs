using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator
{
    public enum Actions
    {
        None,
        Sleep,
        OpenDoor
    }

    public struct InteractableAction
    {
        public Actions action;
        public Vector3 interactableLocation;
        public Interactable currInteractable;

        public InteractableAction(Actions action, Vector3 interactableLocation, Interactable interactable)
        {
            this.action = action;
            this.interactableLocation = interactableLocation;
            this.currInteractable = interactable;
        }
    }

    public struct Action
    {
        public Vector2 walkingVelocity;
        public float jumpVelocity;
        public InteractableAction interactableAction;
        public bool idle;

        public Action(Vector3 walkingVelocity, float jumpVelocity, InteractableAction interactableAction, bool isIdle)
        {
            this.walkingVelocity = walkingVelocity;
            this.jumpVelocity = jumpVelocity;
            this.interactableAction = interactableAction;
            this.idle = isIdle;
        }
    }

    public Action getActionToPerform(GameObject character, Vector3 start, Vector3Int goal, Vector3 additionalForces, Interactable interactable)
    {
        Vector3 dirVec = goal - start;
        float yDiffSquared = dirVec.y * dirVec.y;

        Action currAction = new Action();

        if (yDiffSquared >= 1f && dirVec.y > 0)
        {
            //currAction.jumpVelocity = 1f;
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

        if (interactable != null)
        {
            Vector3 dist = character.transform.position - goal;
            float squaredDistX = dist.x * dist.x;
            float squaredDistY = dist.y * dist.y;
            float squaredDistZ = dist.z * dist.z;
            if ((squaredDistX <= interactable.InteractableRadiusSquared.x) && (squaredDistY <= interactable.InteractableRadiusSquared.y) && (squaredDistZ <= interactable.InteractableRadiusSquared.z))
            {
                currAction.interactableAction = new InteractableAction(Actions.Sleep, interactable.CenterPos, interactable);
            }
        }

        return currAction;
    }
}
