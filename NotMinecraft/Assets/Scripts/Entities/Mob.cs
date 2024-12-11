using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SteeringPipeline))]
public class Mob : MonoBehaviour
{
    [SerializeField] protected float mWalkSpeed = 14f;
    [SerializeField] protected float mMaxJumpPower = 500f;
    protected SteeringPipeline mSteeringPipeline;
    protected Rigidbody mRigidbody;

    public float WalkSpeed
    {
        get
        {
            return mWalkSpeed;
        }
    }

    public float MaxJumpPower
    {
        get
        {
            return mMaxJumpPower;
        }
    }

    private void Awake()
    {
        loadComponents();
    }

    protected void loadComponents()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mSteeringPipeline = GetComponent<SteeringPipeline>();
        mRigidbody.drag = 3;
    }
}
