using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    private bool mOccupied = false;

    public bool Occupied
    {
        get
        {
            return mOccupied;
        }

        set
        {
            mOccupied = value;
        }
    }

    public Bed()
    {
        mInteractableRadiusSquared = new Vector3(1.5f, 1.5f, 1.5f);
        mAction = Actions.Sleep;
    }

    public override bool canVisit()
    {
        return !mOccupied;
    }
}
