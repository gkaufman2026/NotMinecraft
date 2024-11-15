using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SteeringPipeline))]
public class Mob : MonoBehaviour
{
    [SerializeField] private float mWalkSpeed = 100f;
    [SerializeField] private float mMaxJumpPower = 100f;
    private SteeringPipeline mSteeringPipeline;

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

    private void Start()
    {
        mSteeringPipeline = GetComponent<SteeringPipeline>();
        Vector3Int testGoal = new Vector3Int(0, 1, 0);
        mSteeringPipeline.buildPathToGoal(testGoal);
    }

    private void FixedUpdate()
    {
        Actuator.Action currentAction = mSteeringPipeline.getSteering();
        transform.position += new Vector3(currentAction.walkingVelocity.x, currentAction.jumpVelocity, currentAction.walkingVelocity.y);
    }
}
