using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utils;

public static class PathMaker
{
    private static PriorityQueue<AStarNode, float> frontier;

    private struct AStarNode
    {
        public AStarNode(Vector3Int p)
        {
            centerPoint = p;
            weight = 0;
            heuristic = 0;
        }

        public Vector3Int centerPoint;
        public float weight;
        public float heuristic;
        public static bool operator <(AStarNode a, AStarNode b)
        {
            return (a.weight + a.heuristic) > (b.weight + b.heuristic);
        }

        public static bool operator >(AStarNode a, AStarNode b)
        {
            return (a.weight + a.heuristic) < (b.weight + b.heuristic);
        }

        public float getTotalWeight() { return weight + heuristic; }

        public float calculateEuclideanHeuristic(Vector3Int goal)
        {
            return (goal - centerPoint).magnitude;
        }
    }

    public static List<Vector3Int> generatePath(ref World world, Vector3Int startPoint, Vector3Int goalPoint)
    {
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        frontier = new PriorityQueue<AStarNode, float>();
        HashSet<Vector3Int> frontierSet = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, bool> visited = new Dictionary<Vector3Int, bool>();
        int totalNeighborsEvaluated = 0;

        //Sets up start node
        AStarNode start = new AStarNode(startPoint);
        start.heuristic = start.calculateEuclideanHeuristic(goalPoint);
        frontier.Enqueue(start, start.getTotalWeight());
        frontierSet.Add(start.centerPoint);
        Vector3Int endPoint = Vector3Int.zero;

        while (frontier.Count > 0)
        {
            AStarNode currNode = frontier.Dequeue(); //Gets the current node from the priority queue
            frontierSet.Remove(currNode.centerPoint); //Removes the current node point from the fronteirSet

            endPoint = currNode.centerPoint; //Keeps track of last point evaluated
            if (currNode.centerPoint == goalPoint) //Checks if goal and breaks
            {
                endPoint = goalPoint;
                break;
            }

            visited[currNode.centerPoint] = true; //Sets the current point as visited

            //Gest neighbors from gridmask
        //    List<Vector3Int> neighbors = getGridMaskPoints(gridMaskSize, minSegmentSize, noise, currNode.centerPoint, frontierSet, visited); //Gets the neighbors of the current point
        //    totalNeighborsEvaluated += neighbors.Count;
        //    Debug.Log("Ne: " + neighbors.Count);

        //    //While the neighbors exist, update the cameFrom map with them and add them to the priority queue
        //    if (neighbors.Count > 0)
        //    {
        //        foreach (Vector3Int neighbor in neighbors)
        //        {
        //            cameFrom[neighbor] = currNode.centerPoint;
        //            AStarNode newNeighborNode = new AStarNode(neighbor); //Converts neighbor point into AStar node

        //            //Calculates weight by converting positions to a unit cube (since the noise values are from 0-1)
        //            //and then finds the magnatude of the displacment vector which is added to the previous node's weight
        //            Vector2 scaledNeighborXYPos = (Vector2)neighbor / noise.GetLength(0);
        //            Vector2 scaledCurrentXYPos = (Vector2)currNode.centerPoint / noise.GetLength(0);
        //            Vector3 worldDisVec = new Vector3(scaledNeighborXYPos.x, noise[neighbor.y, neighbor.x], scaledNeighborXYPos.y) - new Vector3(scaledCurrentXYPos.x, noise[currNode.centerPoint.y, currNode.centerPoint.x], scaledCurrentXYPos.y);
        //            newNeighborNode.weight = currNode.weight + worldDisVec.magnitude; //Updates neighbor weights

        //            newNeighborNode.heuristic = newNeighborNode.calculateEuclideanHeuristic(goalPoint, noise); //Calculates node heuristic

        //            //Adds neighbor to priority queue and frontier set
        //            frontier.Enqueue(newNeighborNode, newNeighborNode.getTotalWeight());
        //            frontierSet.Add(neighbor);
        //        }
        //    }
        }

        //Debug.Log("Total Neighbors Evaluated: " + totalNeighborsEvaluated);
        List<Vector3Int> path = new List<Vector3Int>(); //Path from the goal to the start

        //if (endPoint != goalPoint)
        //{
        //    Debug.Log("Did not find goal");
        //}
        //else
        //{
        //    Debug.Log("Found Goal!!");
        //}

        ////Builds path only if a goal was found
        //Vector3Int current = endPoint;
        //while (current != start.centerPoint)
        //{ //Runs backwards through cameFrom map to build path from the goal to the start
        //    path.Add(current);
        //    current = cameFrom[current];
        //}

        //path.Add(start.centerPoint); //Adds start to path

        return path;
    }
}
