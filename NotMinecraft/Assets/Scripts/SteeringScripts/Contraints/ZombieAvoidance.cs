using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAvoidance : Constraint
{
    private float mAvoidanceRadius = 5f;
    private Vector3 zombieLocation;
    private bool mMovedAwayLastFrame = false;

    public override bool isViolated(GameObject character, Vector3 startPos, Vector3Int goalPos)
    {
        //bool zombieInPath = false; //Code way to detect if zombie is in the way

        if (mMovedAwayLastFrame)
        {
            mMovedAwayLastFrame = false;
            return false;
        }

        Collider[] overlaps = Physics.OverlapSphere(goalPos, mAvoidanceRadius);
        foreach (Collider collision in overlaps)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                zombieLocation = collision.transform.position;
                return true;
            }
        }

        return false;
    }

    public override Vector3Int suggestNewGoal(GameObject character, Vector3 startPos, Vector3Int goalPos)
    {
        Vector3 directionVec = (goalPos - zombieLocation);
        Vector3 playerDirectionVec = (character.transform.position - zombieLocation).normalized;
        float maxCoordDif = Mathf.Max(directionVec.x, directionVec.z);

        Vector2 avoidanceVec = Vector2.zero;

        if (character.transform.position.x > zombieLocation.x)
        {
            avoidanceVec.x = mAvoidanceRadius + 1;
        }
        else
        {
            avoidanceVec.x -= mAvoidanceRadius + 1;
        }

        if (character.transform.position.z > zombieLocation.z)
        {
            avoidanceVec.y = mAvoidanceRadius + 1;
        }
        else
        {
            avoidanceVec.y -= mAvoidanceRadius + 1;
        }

        Vector3Int newGoal = new Vector3Int(Mathf.RoundToInt(zombieLocation.x), goalPos.y, Mathf.RoundToInt(zombieLocation.z)) + new Vector3Int(Mathf.RoundToInt(avoidanceVec.x), 0, Mathf.RoundToInt(avoidanceVec.y));
        mMovedAwayLastFrame = true; //This is to stop the player from entering deadlock in case the suggested new goal is forced to be too close

        return newGoal;

        /*
         mMovedAwayLastFrame = true;
        Vector3 directionVec = (goalPos - zombieLocation).normalized;
        Vector3 playerDirectionVec = (character.transform.position - zombieLocation).normalized;
        float maxCoordDif = Mathf.Max(directionVec.x, directionVec.z);

        Vector3 avoidanceVec = directionVec * (mAvoidanceRadius);
        Vector3Int newGoal = new Vector3Int(Mathf.RoundToInt(zombieLocation.x), goalPos.y, Mathf.RoundToInt(zombieLocation.z)) + new Vector3Int(Mathf.RoundToInt(avoidanceVec.x), 0, Mathf.RoundToInt(avoidanceVec.z));
        newGoal += new Vector3Int(Mathf.RoundToInt(directionVec.x), 0, Mathf.RoundToInt(directionVec.z)); //This should move one block past radius
        return newGoal;
         */
    }
}
