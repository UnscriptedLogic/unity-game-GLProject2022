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
            UnityEngine.Random.InitState(seed);
            nodes = new List<GridNode>(GridGenerator.GridNodes);
            path = new List<GridNode>();

            GetWeightPoints();
            //path.AddRange(PathFinder.GetPath(weightPoints[0], weightPoints[1]));
            //path.AddRange(PathFinder.GetPath(weightPoints[1], weightPoints[2]));

            StartCoroutine(Something());
        }

        private void GetWeightPoints()
        {
            weightPoints = new GridNode[weightPointCount];
            for (int i = 0; i < weightPointCount; i++)
            {
                int index = UnityEngine.Random.Range(0, GridGenerator.GridNodes.Length - 1);
                weightPoints[i] = nodes[index];
                nodes.RemoveAt(index);

                weightPoints[i].NodeObject.GetComponent<Renderer>().material.color = Color.black;
            }
        }

        private IEnumerator Something()
        {
            for (int i = 0; i < weightPointCount - 1; i++)
            {
                path.AddRange(PathFinder.GetPath(weightPoints[i], weightPoints[i + 1]));
                yield return new WaitForSeconds(0.5f);
            }

            for (int i = 0; i < path.Count; i++)
            {
                path[i].SetColor(Color.white);
            }
        }
    }
}