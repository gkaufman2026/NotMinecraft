using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Mob {
    private bool mIsSleeping = false;
    private float yOffset = 0.5f;
    private bool bedIsTargeted = false;

    public bool IsSleeping
    {
        get { return mIsSleeping; }
    }

    private void Awake()
    {
        loadComponents();
        mSteeringPipeline.addConstraint(new ZombieAvoidance());
    }

    private void Start()
    {
        searchForBed();
    }

    private Nullable<Vector3Int> findNearestBed()
    {
        //Needs to figure how to recalculate this when the thing enters a new chunk (this could also be used for zombies finding villagers)
        return GameManager.instance.WorldRef.GetNearestBedInChunk(new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, (int)gameObject.transform.position.z));
    }

    private void FixedUpdate()
    {
        if (mIsSleeping)
        {
            return;
        }

        Actuator.Action currentAction = mSteeringPipeline.getSteering();

        if (!currentAction.idle)
        {
            switch (currentAction.interactableAction.action)
            {
                case Interactable.Actions.Sleep:
                    gameObject.transform.up = Vector3.right;
                    gameObject.transform.eulerAngles += new Vector3(90, 0, 0);
                    gameObject.transform.position = currentAction.interactableAction.interactableLocation + new Vector3(yOffset, 1, 0 );
                    currentAction.interactableAction.currInteractable.InteractedWith = true;
                    ((Bed)(currentAction.interactableAction.currInteractable)).Occupied = true;
                    mRigidbody.velocity = Vector3.zero;
                    mIsSleeping = true;
                    mRigidbody.isKinematic = true;
                    mSteeringPipeline.UpdatePathVisual();
                    break;
                case Interactable.Actions.None:
                    mRigidbody.AddForce(new Vector3(currentAction.walkingVelocity.x, 0, currentAction.walkingVelocity.y));
                    //mRigidbody.AddForce(new Vector3(0, currentAction.jumpVelocity - mRigidbody.velocity.y, 0));
                    mRigidbody.angularVelocity = Vector3.zero;
                    Vector3 targetDir = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, targetDir, 0.2f);

                    Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
                    Vector3Int intTarget = new Vector3Int(Mathf.RoundToInt(targetDir.x), Mathf.RoundToInt(targetDir.y), Mathf.RoundToInt(targetDir.z));

                    World worldRef = GameManager.instance.WorldRef;
                    BlockType block = worldRef.GetBlockFromWorldCoords(worldRef, intPos + intTarget);
                    if (block != BlockType.AIR && block != BlockType.WATER)
                    {
                        if (mRigidbody.velocity.y < 0.1f && mRigidbody.velocity.y > -0.1f)
                        {
                            mRigidbody.AddForce(new Vector3(0, mMaxJumpPower, 0));
                        }
                    }
                    break;
            }

            //Maybe find a better way to search for beds while a villager is moving
            if (!bedIsTargeted)
            {
                searchForBed();
            }

        }
        else
        {
            if (!searchForBed())
            {
                setRandomTarget();
            }

            mRigidbody.angularVelocity *= 0.9f;
        }
    }

    private bool searchForBed()
    {
        Nullable<Vector3Int> bed = findNearestBed();
        if (bed != null)
        {
            mSteeringPipeline.clearPath();
            setGoal((Vector3Int)bed, GameManager.instance.WorldRef);
            bedIsTargeted = true;
            return true;
        }

        bedIsTargeted = false;
        return false;
    }

    public void setGoal(Vector3Int goal, World world)
    {
        mSteeringPipeline.buildPathToGoal(goal, world);
    }

    public void setRandomTarget()
    {
        //Could make this better such as by picking a new point in the direction the villager is going
        int xVal = UnityEngine.Random.Range(-5, 6) + (int)transform.position.x;
        int zVal = UnityEngine.Random.Range(-5, 6) + (int)transform.position.z;
        ChunkData chunkData = GameManager.instance.WorldRef.GetChunkDataFromWorldCoords(new Vector3Int(xVal, (int)transform.position.y, zVal));

        int surfHeight = GameManager.instance.WorldRef.terrainGenerator.biomeGenerator.GetSurfaceHeightNoise(xVal, zVal, chunkData.chunkHeight);

        Vector3Int newWorldPos = new Vector3Int(xVal, surfHeight + 1, zVal);

        setGoal(newWorldPos, GameManager.instance.WorldRef);
    }
}
