using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Search;
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
            List<Vector3Int> neighbors = getVisitableNeightbors(ref world, currNode.centerPoint, ref frontierSet, ref visited); //Gets the neighbors of the current point
            totalNeighborsEvaluated += neighbors.Count;

            //While the neighbors exist, update the cameFrom map with them and add them to the priority queue
            if (neighbors.Count > 0)
            {
                foreach (Vector3Int neighbor in neighbors)
                {
                    cameFrom[neighbor] = currNode.centerPoint;
                    AStarNode newNeighborNode = new AStarNode(neighbor); //Converts neighbor point into AStar node

                    //Calculates weight by converting positions to a unit cube (since the noise values are from 0-1)
                    //and then finds the magnatude of the displacment vector which is added to the previous node's weight
                    newNeighborNode.weight = currNode.weight + (neighbor - currNode.centerPoint).magnitude; //Updates neighbor weights

                    newNeighborNode.heuristic = newNeighborNode.calculateEuclideanHeuristic(goalPoint); //Calculates node heuristic

                    //Adds neighbor to priority queue and frontier set
                    frontier.Enqueue(newNeighborNode, newNeighborNode.getTotalWeight());
                    frontierSet.Add(neighbor);
                }
            }
        }

        List<Vector3Int> path = new List<Vector3Int>(); //Path from the goal to the start

        if (endPoint != goalPoint)
        {
            Debug.Log("Did not find goal");
        }
        else
        {
            Debug.Log("Found Goal!!");
        }

        //Builds path only if a goal was found
        Vector3Int current = endPoint;
        while (current != start.centerPoint)
        { //Runs backwards through cameFrom map to build path from the goal to the start
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start.centerPoint); //Adds start to path

        return path;
    }

    private static List<Vector3Int> getVisitableNeightbors(ref World world, Vector3Int currPoint, ref HashSet<Vector3Int> frontierSet, ref Dictionary<Vector3Int, bool> visited)
    {
        List<Vector3Int> visitables = new();

        //Finds neighbors around center
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++) {
                    Vector3Int checkPoint = currPoint + new Vector3Int(i, j, k);
                    BlockType currBlockType = world.GetBlockFromWorldCoords(world, checkPoint);
                    BlockType currBlockBelowType = world.GetBlockFromWorldCoords(world, checkPoint + Vector3Int.down);
                    if (!frontierSet.Contains(checkPoint) && !visited.ContainsKey(checkPoint) && (currBlockType == BlockType.AIR || currBlockType == BlockType.WATER) && checkPoint != currPoint)
                    {
                        if (currBlockBelowType != BlockType.AIR && currBlockBelowType != BlockType.WATER)
                        {
                            visitables.Add(checkPoint);
                        }
                    }
                }
            }
        }

        return visitables; //Returns neighbors found
    }
}
