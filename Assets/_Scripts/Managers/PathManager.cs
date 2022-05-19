using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;

namespace Core.Pathing
{
    public static class PathManager
    {
        private static int seed = 10; //The seed for randomization
        private static int weightPointCount = 5; //The amount of 'crucial' points the path needs to meet (for proper path spread across the map)

        private static GridNode[] weightPoints;
        private static List<GridNode> nodes;

        public static void GeneratePath()
        {
            UnityEngine.Random.InitState(seed);
            nodes = new List<GridNode>(GridGenerator.GridNodes);

            GetWeightPoints();
        }

        private static void GetWeightPoints()
        {
            weightPoints = new GridNode[weightPointCount];
            for (int i = 0; i < weightPointCount; i++)
            {
                int index = UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1);
                weightPoints[i] = nodes[index];
                weightPoints[i].NodeObject.SetActive(false);
                nodes.RemoveAt(index);
            }
        }
    }
}