using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SteeringPipeline : MonoBehaviour
{
    [SerializeField] private Material _mLineVisualMaterial;
    Targeter targeter = new(); //For determining the target action/goal
    Decomposer decomposer = new(); //For determining what sub goal in the path to go to
    List<Constraint> constraints = new(); //For determining if a movement is invalid and fix movment
    Actuator actuator = new(); //For determining the actual movement of the character
    private Vector3 constraintForces;

    private int mConstraintSteps = 2;

    public void buildPathToGoal(Vector3Int goal, World world)
    {
        targeter.addGoal(goal);

        Vector3Int startPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        decomposer.buildAndAddNextPath(ref world, startPos, goal);

        UpdatePathVisual();
    }

    public void UpdatePathVisual()
    {
        //Clears previous line renderers
        LineRenderer[] lineRenderers = gameObject.GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            Destroy(lr.gameObject);
        }

        //Runs through each point and makes a line between them, adding up total path length
        List<Vector3Int> path = decomposer.getCurrPath();
        float totalLength = 0;
        if (path.Count > 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                GameObject empty = new GameObject();
                empty.transform.parent = gameObject.transform;
                LineRenderer line = empty.AddComponent<LineRenderer>();

                //Converts points into world space
                Vector3 pos1 = new Vector3(path[i].x, path[i].y, path[i].z);
                Vector3 pos2 = new Vector3(path[i + 1].x, path[i + 1].y, path[i + 1].z);
                totalLength += (pos2 - pos1).magnitude;
                line.enabled = true;
                empty.transform.position = pos1;
                line.SetPosition(0, pos1);
                line.SetPosition(1, pos2);
                line.startWidth = 0.5f;
                line.endWidth = 0.5f;

                if (_mLineVisualMaterial != null)
                {
                    line.material = _mLineVisualMaterial;
                }
            }
        }

        if (path.Count > 0)
        {
            GameObject finalLine = new GameObject();
            finalLine.transform.parent = gameObject.transform;
            LineRenderer lineLast = finalLine.AddComponent<LineRenderer>();
            Vector3 pos2Path = new Vector3(path[path.Count - 1].x, path[path.Count - 1].y, path[path.Count - 1].z);
            lineLast.SetPosition(0, gameObject.transform.position);
            lineLast.SetPosition(1, pos2Path);
            lineLast.startWidth = 0.5f;
            lineLast.endWidth = 0.5f;

            if (_mLineVisualMaterial != null)
            {
                lineLast.material = _mLineVisualMaterial;
            }
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
                constraintForces = Vector3.zero;
                for (int i = 0; i < mConstraintSteps; i++)
                { 
                    foreach (Constraint constraint in constraints)
                    {
                        if (constraint.isViolated(gameObject, startPos, (Vector3Int)currGoal))
                        {
                            constraintForces += constraint.suggestNewGoal(gameObject, startPos, (Vector3Int)currGoal);

                            ////This is for when the constraint used to return the new subGoal
                            //if (currGoal != null)
                            //{
                            //    decomposer.setCurrSubGoal((Vector3Int)currGoal);
                            //}

                            //UpdatePathVisual();
                            goto skipReturn;
                        }
                    }

                    UpdatePathVisual();
                    return actuator.getActionToPerform(gameObject, startPos, (Vector3Int)currGoal, constraintForces, decomposer.CurrInteractable);
                    skipReturn: { }
                }
            }
            else
            {
                targeter.achievedCurrTarget();
            }
        }

        //Debug.Log("DONE!!");
        //Actuator.Action defaultAction = new Actuator.Action(Vector3.zero, 0, new Actuator.InteractableAction(Actuator.Actions.None, Vector3Int.zero), true);
        Actuator.Action defaultAction = new Actuator.Action();
        defaultAction.walkingVelocity = Vector2.zero;
        defaultAction.idle = true;
     
        return defaultAction;
    }

    public void addConstraint(Constraint constraint)
    {
        constraints.Add(constraint);
    }

    public void clearPath()
    {
        targeter.clearGoals();
        decomposer.clearPath();
    }

    public void SetSubGoalSatisfactoryRadius(float x, float y, float z)
    {
        decomposer.SetSubGoalSatisfactoryRadius(x, y, z);
    }

    public void SetGoalSatisfactoryRadius(float x, float y, float z)
    {
        decomposer.SetGoalSatisfactoryRadius(x, y, z);
    }
}
