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
    private bool paused = false;

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
                    mVillagerChasing = null;
                }
            }

            switch (currentAction.interactableAction.action)
            {
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

        }
        else
        {
            if (!paused)
            {
                handleIdleBehavior();
            }
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

    public bool pathfindToVillager()
    {
        Nullable<Vector3Int> villager = findNearestVillager();
        if (villager != null)
        {
            setGoal((Vector3Int)villager, GameManager.instance.WorldRef);
            return true;
        }

        return false;
    }

    private void handleIdleBehavior()
    {
        if (!pathfindToVillager()) //Optomize this to not happen every update probobly
        {
            setRandomTarget();
        }

        float velMag = mRigidbody.velocity.sqrMagnitude;
        mRigidbody.angularVelocity *= 0.9f;
    }

    public void setRandomTarget()
    {
        int delay = UnityEngine.Random.Range(0, 2);
        if (delay == 0)
        {
            paused = true;
            mSteeringPipeline.UpdatePathVisual();
            StartCoroutine(Co_StandStillDelay(UnityEngine.Random.Range(1, 4)));
            return;
        }

        int xVal = UnityEngine.Random.Range(-5, 6) + (int)transform.position.x;
        int zVal = UnityEngine.Random.Range(-5, 6) + (int)transform.position.z;
        ChunkData chunkData = GameManager.instance.WorldRef.GetChunkDataFromWorldCoords(new Vector3Int(xVal, (int)transform.position.y, zVal));

        int surfHeight = GameManager.instance.WorldRef.terrainGenerator.biomeGenerator.GetSurfaceHeightNoise(xVal, zVal, chunkData.chunkHeight);

        Vector3Int newWorldPos = new Vector3Int(xVal, surfHeight + 1, zVal);

        setGoal(newWorldPos, GameManager.instance.WorldRef);
    }

    private IEnumerator Co_StandStillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        handleIdleBehavior();
        paused = false;
    }
}
