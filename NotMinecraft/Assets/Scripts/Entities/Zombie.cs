using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Zombie : Mob
{
    private float mSearchRadius = 10f;
    private Villager mVillagerChasing = null;

    private void Start()
    {
        pathfindToVillager();
    }

    private void FixedUpdate()
    {
        Actuator.Action currentAction = mSteeringPipeline.getSteering();

        if (!currentAction.idle)
        {
            if (mVillagerChasing != null) //Optomize this to not call every update
            {
                if (mVillagerChasing.IsSleeping)
                {
                    mSteeringPipeline.clearPath();
                }
            }

            switch (currentAction.interactableAction.action)
            {
                case Actuator.Actions.None:
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

        }
        else
        {
            pathfindToVillager(); //Optomize this to not happen every update probobly
            float velMag = mRigidbody.velocity.sqrMagnitude;
            mRigidbody.angularVelocity *= 0.9f;
        }
    }

    private Nullable<Vector3Int> findNearestVillager()
    {
        Dictionary<Vector3Int, Villager> villagersScripts = new Dictionary<Vector3Int, Villager>();
        List<Vector3Int> villagers = new List<Vector3Int>();
        Collider[] overlaps = Physics.OverlapSphere(transform.position, mSearchRadius);
        foreach (Collider collision in overlaps)
        {
            if (collision.gameObject.tag == "Villager")
            {
                Villager villagerScript = collision.gameObject.GetComponent<Villager>();
                if (villagerScript != null && !villagerScript.IsSleeping)
                {
                    Vector3Int intPos = new Vector3Int((int)collision.transform.position.x, (int)collision.transform.position.y, (int)collision.transform.position.z);
                    villagersScripts.Add(intPos, collision.gameObject.GetComponent<Villager>());
                    villagers.Add(intPos);
                }
            }
        }

        float distance = mSearchRadius * mSearchRadius + 2;
        Vector3Int closestVillager = Vector3Int.zero;
        Vector3Int zombiePos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        foreach (Vector3Int villager in villagers)
        {
            Vector3Int vecTo = villager - zombiePos;
            float sqrtMag = vecTo.sqrMagnitude;
            if (sqrtMag < distance)
            {
                distance = sqrtMag;
                closestVillager = villager;
            }
        }

        if (distance != mSearchRadius * mSearchRadius + 2)
        {
            mVillagerChasing = villagersScripts[closestVillager];
            return closestVillager;
        }

        return null;
    }

    public void setGoal(Vector3Int goal, World world)
    {
        mSteeringPipeline.buildPathToGoal(goal, world);
    }

    public void pathfindToVillager()
    {
        Nullable<Vector3Int> villager = findNearestVillager();
        if (villager != null)
        {
            setGoal((Vector3Int)villager, GameManager.instance.WorldRef);
        }
    }
}
