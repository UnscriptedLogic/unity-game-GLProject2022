using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;
using System.Threading.Tasks;

namespace Core.Pathing
{
    public class PathManager : MonoBehaviour
    {
        private GridNode[] weightPoints;
        private List<GridNode> nodes;
        private List<GridNode> path;
        private MapManager mapManager;

        public GridNode[] Path { get => path.ToArray(); }

        public void Initialize(MapManager mapManager)
        {
            this.mapManager = mapManager;
        }

        public void GeneratePath(Action callback)
        {
            nodes = new List<GridNode>(GridGenerator.GridNodes);
            path = new List<GridNode>();

            GetWeightPoints();
            StartCoroutine(StitchPaths(callback));
        }

        public void GeneratePath(Action callback, int seed)
        {
            UnityEngine.Random.InitState(seed);
            nodes = new List<GridNode>(GridGenerator.GridNodes);
            path = new List<GridNode>();

            GetWeightPoints();
            StartCoroutine(StitchPaths(callback));
        }

        private void GetWeightPoints()
        {
            List<GridNode> newWeights = new List<GridNode>();
            newWeights.Add(nodes[UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1)]);
            int retries = 0, maxRetries = 50;
            while (retries < maxRetries && newWeights.Count < mapManager.WPcount)
            {
                int randomIndex = UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1);
                bool isClose = false;
                for (int j = 0; j < newWeights.Count; j++)
                {
                    //if (PathFinder.GetDistance(nodes[randomIndex], newWeights[j]) < weightPointDistance)
                    if (Vector3.Distance(nodes[randomIndex].NodeObject.transform.position, newWeights[j].NodeObject.transform.position) < mapManager.WPdist)
                    {
                        isClose = true;
                        break;
                    }
                }

                retries++;
                if (!isClose)
                {
                    newWeights.Add(nodes[randomIndex]);
                    retries = 0;
                }
            }

            weightPoints = newWeights.ToArray();
        }

        private IEnumerator StitchPaths(Action callback)
        {
            for (int i = 0; i < mapManager.WPcount - 1; i++)
            {
                if (i + 1 < weightPoints.Length)
                {
                    path.AddRange(PathFinder.GetPath(weightPoints[i], weightPoints[i + 1], mapManager.AllowOverlap));
                    yield return new WaitForSeconds(0.25f);
                }
            }

            for (int i = 0; i < path.Count; i++)
            {
                path[i].SetVisibility(false);
            }

            callback();
        }
    }
}