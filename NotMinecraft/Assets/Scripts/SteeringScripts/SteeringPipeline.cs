using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SteeringPipeline : MonoBehaviour
{
    Targeter targeter = new(); //For determining the target action/goal
    Decomposer decomposer = new(); //For determining what sub goal in the path to go to
    List<Constraint> constraints = new(); //For determining if a movement is invalid and fix movment
    Actuator actuator = new(); //For determining the actual movement of the character

    private int mConstraintSteps = 2;

    public void buildPathToGoal(Vector3Int goal, ref World world)
    {
        targeter.addGoal(goal);

        Vector3Int startPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        decomposer.buildAndAddNextPath(ref world, startPos, goal);

        //Clears previous line renderers
        LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            Destroy(lr.gameObject);
        }

        //Runs through each point and makes a line between them, adding up total path length
        List<Vector3Int> path = decomposer.getCurrPath();
        float totalLength = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {

            GameObject empty = new GameObject();
            LineRenderer line = empty.AddComponent<LineRenderer>();

            //Converts points into world space
            Vector3 pos1 = new Vector3(path[i].x, path[i].y, path[i].z);
            Vector3 pos2 = new Vector3(path[i + 1].x, path[i + 1].y, path[i+1].z);
            totalLength += (pos2 - pos1).magnitude;
            line.enabled = true;
            empty.transform.position = pos1;
            line.SetPosition(0, pos1);
            line.SetPosition(1, pos2);
        }
    }

    public void UpdatePathVisual()
    {
        //Clears previous line renderers
        LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            Destroy(lr.gameObject);
        }

        //Runs through each point and makes a line between them, adding up total path length
        List<Vector3Int> path = decomposer.getCurrPath();
        float totalLength = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {

            GameObject empty = new GameObject();
            LineRenderer line = empty.AddComponent<LineRenderer>();

            //Converts points into world space
            Vector3 pos1 = new Vector3(path[i].x, path[i].y, path[i].z);
            Vector3 pos2 = new Vector3(path[i + 1].x, path[i + 1].y, path[i + 1].z);
            totalLength += (pos2 - pos1).magnitude;
            line.enabled = true;
            empty.transform.position = pos1;
            line.SetPosition(0, pos1);
            line.SetPosition(1, pos2);
        }
    }

    public Actuator.Action getSteering()
    {
        Nullable<Vector3Int> currGoal = targeter.getCurrentTarget();

        if (currGoal != null )
        {
            currGoal = decomposer.getCurrSubGoal(transform.position); 

            if (currGoal != null)
            {
                Vector3 startPos = transform.position;
                for (int i = 0; i < mConstraintSteps; i++)
                { 
                    foreach (Constraint constraint in constraints)
                    {
                        if (constraint.isViolated(gameObject, startPos, (Vector3Int)currGoal))
                        {
                            currGoal = constraint.suggestNewGoal(gameObject, startPos, (Vector3Int)currGoal); //Make sure that this changes sub goal and also handles player position

                            if (currGoal != null)
                            {
                                decomposer.setCurrSubGoal((Vector3Int)currGoal);
                            }

                            UpdatePathVisual();
                            goto skipReturn;
                        }
                    }

                    return actuator.getActionToPerform(gameObject, startPos, (Vector3Int)currGoal);
                    skipReturn: { }
                }
            }
            else
            {
                targeter.achievedCurrTarget();
            }
        }

        Debug.Log("DONE!!");
        Actuator.Action defaultAction = new Actuator.Action(Vector3.zero, 0, false, true);
        return defaultAction;
    }

    public void addConstraint(Constraint constraint)
    {
        constraints.Add(constraint);
    }
}
