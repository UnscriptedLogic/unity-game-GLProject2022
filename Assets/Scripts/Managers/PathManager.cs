using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;
using System.Threading.Tasks;
using System.Threading;

namespace Core.Pathing
{
    public class PathManager : MonoBehaviour
    {
        [SerializeField] private int seed = 10; //The seed for randomization
        [SerializeField] private int weightPointCount = 5; //The amount of 'crucial' points the path needs to meet (for proper path spread across the map)
        [SerializeField] private float weightPointDistance = 5f; //The amount of distance between the weightpoints;
        [SerializeField] private bool randomizeSeed = false;
        [SerializeField] private bool allowOverlap = false;

        private GridNode[] weightPoints;
        private List<GridNode> nodes;
        private List<GridNode> path;
        private GridManager gridManager;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public GridNode[] Path { get => path.ToArray(); }
        public CancellationTokenSource TokenSource => tokenSource;

        public void Initialize(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        public async Task GeneratePath()
        {
            if (randomizeSeed)
            {
                if (!GameManager.setSeed)
                {
                    GameManager.seed = UnityEngine.Random.Range(-1000000, 1000000);
                }

                seed = GameManager.seed;
                GameManager.setSeed = false;
            }

            bool invalid = true;
            int counter = 0;

            while (invalid)
            {
                if (tokenSource.IsCancellationRequested)
                {
                    return;
                }

                UnityEngine.Random.InitState(seed);
                nodes = new List<GridNode>(GridGenerator.GridNodes);
                path = new List<GridNode>();

                await CreatePath(tokenSource);

                for (int i = 0; i < path.Count; i++)
                {
                    int adjacentPaths = 0; 
                    GridNode[] neighbours = GridGenerator.GetNeighboursOf(path[i]);
                    for (int j = 0; j < neighbours.Length; j++)
                    {
                        if (neighbours[j] != null)
                        {
                            if (neighbours[j].isObstacle)
                            {
                                adjacentPaths++;
                            }
                        }
                    }

                    float percentage = (path.Count / (float)GridGenerator.GridNodes.Length) * 100f;
                    if (adjacentPaths > 6 || percentage < 17.5f)
                    {
                        invalid = true;
                        counter++;
                        Reseed(nodes);
                        break;
                    }
                    else
                    {
                        invalid = false;
                    }
                }
            }
        }

        private void Reseed(List<GridNode> nodes)
        {
            seed++;
            for (int j = 0; j < nodes.Count; j++)
            {
                nodes[j].isObstacle = false;
                nodes[j].SetVisibility(true);
            }

            Debug.Log("Hello World");

        }

        private void GetWeightPoints()
        {
            List<GridNode> newWeights = new List<GridNode>();
            newWeights.Add(nodes[UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1)]);
            int retries = 0, maxRetries = 50;
            while (retries < maxRetries && newWeights.Count < weightPointCount)
            {
                int randomIndex = UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1);
                bool isClose = false;
                for (int j = 0; j < newWeights.Count; j++)
                {
                    //if (PathFinder.GetDistance(nodes[randomIndex], newWeights[j]) < weightPointDistance)
                    if (Vector3.Distance(nodes[randomIndex].NodeObject.transform.position, newWeights[j].NodeObject.transform.position) < weightPointDistance)
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

        private async Task CreatePath(CancellationTokenSource tokenSource)
        {
            GetWeightPoints();

            for (int i = 0; i < weightPointCount - 1; i++)
            {
                if (tokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (i + 1 < weightPoints.Length)
                {
                    List<GridNode> subpath = PathFinder.GetPath(weightPoints[i], weightPoints[i + 1], allowOverlap);
                    await Task.Yield();

                    if (subpath != null)
                    {
                        path.AddRange(subpath);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < path.Count; i++)
            {
                path[i].SetVisibility(false);
            }
        }

        private void OnDisable()
        {
            tokenSource.Cancel();
        }
    }
}