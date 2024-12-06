using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAvoidance : Constraint
{
    private float mAvoidanceRadius = 5f;
    private Vector3 mZombieLocation;
    private bool mMovedAwayLastFrame = false;

    public override bool isViolated(GameObject character, Vector3 startPos, Vector3Int goalPos)
    {
        //bool zombieInPath = false; //Code way to detect if zombie is in the way

        if (mMovedAwayLastFrame)
        {
            mMovedAwayLastFrame = false;
            return false;
        }

        Collider[] overlaps = Physics.OverlapSphere(character.transform.position, mAvoidanceRadius);
        foreach (Collider collision in overlaps)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                mZombieLocation = collision.transform.position;
                return true;
            }
        }

        return false;
    }

    public override Vector3 suggestNewGoal(GameObject character, Vector3 startPos, Vector3Int goalPos)
    {
        Vector3 forceVec = Vector3.zero;
        mMovedAwayLastFrame = true;
        Rigidbody charRb = character.GetComponent<Rigidbody>();
        Mob mobScript = character.GetComponent<Mob>();
        if (charRb != null)
        {
            Vector3 zomToChar = character.transform.position - mZombieLocation;
            Vector3 charToGoal = character.transform.position - goalPos;

            if (Vector3.Dot(zomToChar.normalized, charToGoal.normalized) > 0.5f)
            {
                zomToChar = new Vector3(zomToChar.z, zomToChar.y, -zomToChar.x);
            }

            forceVec = zomToChar.normalized * mobScript.WalkSpeed * 2;
        }

        return new Vector3(forceVec.x, 0, forceVec.z);
        
        /* mMovedAwayLastFrame = true;
        Vector3 directionVec = (goalPos - zombieLocation).normalized;
        Vector3 playerDirectionVec = (character.transform.position - zombieLocation).normalized;
        float maxCoordDif = Mathf.Max(playerDirectionVec.x, playerDirectionVec.z);

        Vector2 avoidanceVec = Vector2.zero;

        if (maxCoordDif == playerDirectionVec.x)
        {
            if (character.transform.position.x > zombieLocation.x)
            {
                float relativeY = goalPos.z - zombieLocation.z;
                avoidanceVec.x = (Mathf.Sqrt(mAvoidanceRadius * mAvoidanceRadius - relativeY * relativeY) + 1);
                avoidanceVec.y = 0;
            }
            else
            {
                float relativeY = goalPos.z - zombieLocation.z;
                avoidanceVec.x = -(Mathf.Sqrt(mAvoidanceRadius * mAvoidanceRadius - relativeY * relativeY) + 1);
                avoidanceVec.y = 0;
            }
        }
        else
        {
            if (character.transform.position.z > zombieLocation.z)
            {
                float relativeX = goalPos.x - zombieLocation.x;
                avoidanceVec.y = (Mathf.Sqrt(mAvoidanceRadius * mAvoidanceRadius - relativeX * relativeX) + 1);
                avoidanceVec.x = 0;
            }
            else
            {
                float relativeX = goalPos.x - zombieLocation.x;
                avoidanceVec.y = -(Mathf.Sqrt(mAvoidanceRadius * mAvoidanceRadius - relativeX * relativeX) + 1);
                avoidanceVec.x = 0;
            }
        }

        Vector3Int newGoal = new Vector3Int(Mathf.RoundToInt(zombieLocation.x), goalPos.y, Mathf.RoundToInt(zombieLocation.z)) + new Vector3Int(Mathf.RoundToInt(avoidanceVec.x), 0, Mathf.RoundToInt(avoidanceVec.y));
        return newGoal;*/
    }
}
