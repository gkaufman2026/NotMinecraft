using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringPipeline : MonoBehaviour
{
    [SerializeField] private World world;

    Targeter targeter = new(); //For determining the target action/goal
    Decomposer decomposer = new(); //For determining what sub goal in the path to go to
    List<Constraint> constraints = new(); //For determining if a movement is invalid and fix movment
    Actuator actuator = new(); //For determining the actual movement of the character

    private int mConstraintSteps = 2;

    public void buildPathToGoal(Vector3Int goal)
    {
        targeter.addGoal(goal);

        Vector3Int startPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        decomposer.buildAndAddNextPath(ref world, startPos, goal);
    }

    public Actuator.Action getSteering()
    {
        Nullable<Vector3Int> currGoal = targeter.getCurrentTarget();

        if (currGoal != null )
        {
            currGoal = decomposer.getCurrSubGoal(transform.position); //Change this to maybe only have one path at a time and handle path creation and destruction here
            if (currGoal != null)
            {
                Vector3Int startPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
                for (int i = 0; i < mConstraintSteps; i++)
                { 
                    foreach (Constraint constraint in constraints)
                    {
                        if (constraint.isViolated(gameObject, startPos, (Vector3Int)currGoal))
                        {
                            currGoal = constraint.suggestNewGoal(gameObject, startPos, (Vector3Int)currGoal);
                            continue;
                        }
                    }

                    //Ensure the continue doesn't only exit the second for loop (ensure it skips the current top level for loop iteration)
                    return actuator.getActionToPerform(gameObject, startPos, (Vector3Int)currGoal);
                }
            }
            else
            {
                targeter.achievedCurrTarget();
            }
        }

        Actuator.Action defaultAction = new Actuator.Action(Vector3.zero, 0, false);
        return defaultAction;
    }

    private void Start()
    {
        Vector3Int testGoal = new Vector3Int(0, 1, 0);
        buildPathToGoal(testGoal);
    }

    private void FixedUpdate()
    {
        Actuator.Action currentAction = getSteering();
        transform.position += currentAction.walkingVelocity + new Vector3(0, currentAction.jumpVelocity, 0);
    }
}
