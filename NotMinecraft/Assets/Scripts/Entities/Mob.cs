using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SteeringPipeline))]
public class Mob : MonoBehaviour
{
    [SerializeField] private float mWalkSpeed = 1f;
    [SerializeField] private float mMaxJumpPower = 100f;
    private SteeringPipeline mSteeringPipeline;
    private Rigidbody mRigidbody;

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
        mRigidbody = GetComponent<Rigidbody>();
        mSteeringPipeline = GetComponent<SteeringPipeline>();
        mRigidbody.drag = 3;
    }

    private void Start()
    {
        //World t = new World();
        //setGoal(new Vector3Int(0, 2, 0), ref t);
    }

    private void FixedUpdate()
    {
        Actuator.Action currentAction = mSteeringPipeline.getSteering();

        if (!currentAction.idle)
        {
            mRigidbody.AddForce(new Vector3(currentAction.walkingVelocity.x, 0, currentAction.walkingVelocity.y));
            mRigidbody.AddForce(new Vector3(0, currentAction.jumpVelocity - mRigidbody.velocity.y, 0));
            mRigidbody.angularVelocity = Vector3.zero;
            Vector3 target = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
            transform.forward = Vector3.Lerp(transform.forward, target, 0.2f);
        }
        else
        {
            float velMag = mRigidbody.velocity.sqrMagnitude;
            mRigidbody.angularVelocity *= 0.9f;
        }

        //mRigidbody.velocity += new Vector3(currentAction.walkingVelocity.x, 0, currentAction.walkingVelocity.y);

        //if (mRigidbody.velocity.magnitude > WalkSpeed)
        //{
        //    Vector3 normVec = mRigidbody.velocity.normalized;
        //    mRigidbody.velocity = normVec * WalkSpeed;
        //}
    }

    public void setGoal(Vector3Int goal, ref World world)
    {
        mSteeringPipeline.buildPathToGoal(goal, ref world);
    }
}
