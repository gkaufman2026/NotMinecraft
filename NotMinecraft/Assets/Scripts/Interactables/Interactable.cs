using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable
{
    public enum Actions
    {
        None,
        Sleep,
        OpenDoor
    }

    protected Vector3 mCenterPos = Vector3Int.zero;
    protected Vector3 mInteractableRadiusSquared = Vector3Int.zero;
    protected bool mInteractedWith = false;
    protected Actions mAction = Actions.None;

    public Vector3 InteractableRadiusSquared
    {
        get { return mInteractableRadiusSquared; }
    }

    public Vector3 CenterPos
    {
        get { return mCenterPos; }
        set { mCenterPos = value; }
    }

    public bool InteractedWith { 
        get { return mInteractedWith; } 
        set { mInteractedWith = value; }
    }

    public Actions Action 
    { 
        get { return mAction; }
    }

    virtual public bool canVisit()
    {
        return !mInteractedWith;
    }
}
