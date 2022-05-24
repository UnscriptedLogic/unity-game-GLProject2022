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
        [SerializeField] private int seed = 10; //The seed for randomization
        [SerializeField] private int weightPointCount = 5; //The amount of 'crucial' points the path needs to meet (for proper path spread across the map)
        [SerializeField] private float weightPointDistance = 5f; //The amount of distance between the weightpoints;
        [SerializeField] private bool randomizeSeed = false;

        private GridNode[] weightPoints;
        private List<GridNode> nodes;
        private List<GridNode> path;

        public static PathManager instance;
        private void Awake() => instance = this;

        public void Initialize(int randomSeed, int pointCount)
        {
            seed = randomSeed;
            weightPointCount = pointCount;
        }

        public void GeneratePath()
        {
            if (randomizeSeed)
                seed = (int)System.DateTime.Now.TimeOfDay.TotalSeconds;

            UnityEngine.Random.InitState(seed);
            nodes = new List<GridNode>(GridGenerator.GridNodes);
            path = new List<GridNode>();

            GetWeightPoints();

            for (int i = 0; i < weightPoints.Length; i++)
            {
                weightPoints[i].NodeObject.GetComponent<Renderer>().material.color = Color.black;
            }

            //path.AddRange(PathFinder.GetPath(weightPoints[0], weightPoints[1]));
            //path.AddRange(PathFinder.GetPath(weightPoints[1], weightPoints[2]));

            StartCoroutine(Something());
        }

        private void GetWeightPoints()
        {
            //weightPoints = new GridNode[weightPointCount];
            //weightPoints[0] = nodes[UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1)];
            //for (int i = 1; i < weightPointCount; i++)
            //{
            //    int randomIndex = UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1);
            //    weightPoints[i] = nodes[randomIndex];

            //    weightPoints[i].NodeObject.GetComponent<Renderer>().material.color = Color.black;
            //}

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

        private IEnumerator Something()
        {
            for (int i = 0; i < weightPointCount - 1; i++)
            {
                if (i + 1 < weightPoints.Length)
                {
                    path.AddRange(PathFinder.GetPath(weightPoints[i], weightPoints[i + 1]));
                    yield return new WaitForSeconds(0.25f);
                }
            }

            for (int i = 0; i < path.Count; i++)
            {
                path[i].SetColor(Color.white);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3.zero, weightPointDistance);
        }
    }
}