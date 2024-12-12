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
        if (mMovedAwayLastFrame)
        {
            mMovedAwayLastFrame = false;
            return false;
        }

        Collider[] overlaps = Physics.OverlapSphere(character.transform.position, mAvoidanceRadius);
        int count = 0;
        mZombieLocation = Vector3.zero;
        foreach (Collider collision in overlaps)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                mZombieLocation += collision.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            mZombieLocation /= count;
            return true;
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

            if (Vector3.Dot(zomToChar.normalized, charToGoal.normalized) > 0.8f)
            {
                zomToChar = new Vector3(zomToChar.z, zomToChar.y, -zomToChar.x);
            }

            forceVec = 1 * mobScript.WalkSpeed * zomToChar.normalized;
        }

        return new Vector3(forceVec.x, 0, forceVec.z);
    }
}
