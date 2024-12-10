using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable
{
    protected Vector3 mCenterPos = Vector3Int.zero;
    protected Vector3 mInteractableRadiusSquared = Vector3Int.zero;
    protected bool mInteractedWith = false;

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
}
