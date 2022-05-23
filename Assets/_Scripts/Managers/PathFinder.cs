using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;

namespace Core.Pathing
{
    public static class PathFinder
    {
        private static List<GridNode> openSet;
        private static List<GridNode> closedSet;

        public static List<GridNode> GetPath(GridNode startNode, GridNode endNode)
        {
            for (int i = 0; i < GridGenerator.GridNodes.Length; i++)
            {
                GridGenerator.GridNodes[i].gCost = float.MaxValue;
                GridGenerator.GridNodes[i].hCost = 0f;
                GridGenerator.GridNodes[i].cameFromNode = null;
            }

            openSet = new List<GridNode>() { startNode };
            closedSet = new List<GridNode>();

            startNode.gCost = 0;
            startNode.hCost = GetDistance(startNode, endNode);

            while (openSet.Count > 0)
            {
                GridNode currentNode = GetLowestFCost(openSet);
                currentNode.isObstacle = true;

                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (GridNode neighbourNode in GetNeighbours(currentNode))
                {
                    //if (neighbourNode == null)
                    //    UnityEngine.Debug.Log(currentNode.NodeObject.name);

                    if (closedSet.Contains(neighbourNode)) continue;

                    float tmpGcost = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                    if (tmpGcost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tmpGcost;
                        neighbourNode.hCost = GetDistance(neighbourNode, endNode);

                        if (!openSet.Contains(neighbourNode))
                        {
                            openSet.Add(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        private static List<GridNode> GetNeighbours(GridNode node)
        {
            List<GridNode> neighbours = new List<GridNode>();

            if (node.Coords.x - 1 >= 0)
            {
                neighbours.Add(GridGenerator.GetNodeAt(node.Coords.x - 1, node.Coords.y));
            }

            if (node.Coords.y - 1 >= 0)
            {
                neighbours.Add(GridGenerator.GetNodeAt(node.Coords.x, node.Coords.y - 1));
            }

            if (node.Coords.x + 1 < GridGenerator.GridSize.x)
            {
                neighbours.Add(GridGenerator.GetNodeAt(node.Coords.x + 1, node.Coords.y));
            }

            if (node.Coords.y + 1 < GridGenerator.GridSize.y)
            {
                neighbours.Add(GridGenerator.GetNodeAt(node.Coords.x, node.Coords.y + 1));
            }

            return neighbours;
        }

        private static List<GridNode> CalculatePath(GridNode endNode)
        {
            List<GridNode> path = new List<GridNode>();
            path.Add(endNode);
            GridNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();

            UnityEngine.Debug.Log("done");

            return path;
        }

        public static float GetDistance(GridNode nodeA, GridNode nodeB)
        {
            //int distX = (int)MathF.Abs(nodeA.Coords.x - nodeB.Coords.x);
            //int distY = (int)MathF.Abs(nodeA.Coords.y - nodeB.Coords.y);

            //if (distX > distY)
            //    return (14 * distY) + 10 * (distX - distY);

            //return (14 * distX) + 10 * (distY - distX);
            return Vector3.Distance(nodeB.NodeObject.transform.position, nodeA.NodeObject.transform.position);
        }

        private static GridNode GetLowestFCost(List<GridNode> gridNodes)
        {
            GridNode lowestNode = gridNodes[0];
            for (int i = 1; i < gridNodes.Count; i++)
            {
                if (gridNodes[i].fCost < lowestNode.fCost)
                {
                    lowestNode = gridNodes[i];
                }
            }

            return lowestNode;
        }
    }
}