using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Grid
{
    public static class GridGenerator
    {
        private static Vector2Int staticGridSize;
        private static GridNode[] staticGridNodes;
        private static Dictionary<Vector2Int, GridNode> coordDictionary;

        public static GridNode[] GridNodes { get => staticGridNodes; }
        public static Dictionary<Vector2Int, GridNode> CoordDictionary { get => coordDictionary; }

        public static GridNode[] CreateGrid(Vector2Int gridSize, float spacing, Vector3 center, GameObject prefab, int teamIndex, Transform parent = null)
        {
            staticGridSize = gridSize;
            coordDictionary = new Dictionary<Vector2Int, GridNode>();
            staticGridNodes = new GridNode[gridSize.x * gridSize.y];

            int counter = 0;
            GridScaffold(center, gridSize, spacing, (x, y, pos) =>
            {
                GameObject nodeObject = UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity, parent);
                nodeObject.name = $"{x}, {y}";
                GridNode node = new GridNode(x, y, nodeObject, teamIndex);
                coordDictionary.Add(new Vector2Int(x, y), node);
                staticGridNodes[counter] = node;
                counter++;
            });

            return staticGridNodes;
        }

        public static GridNode[] CreateGrid(Vector2Int gridSize, float spacing, Vector3 center, GameObject prefab, int teamIndex, Action callback, Transform parent = null)
        {
            staticGridSize = gridSize;
            coordDictionary = new Dictionary<Vector2Int, GridNode>();
            staticGridNodes = new GridNode[gridSize.x * gridSize.y];

            int counter = 0;
            GridScaffold(center, gridSize, spacing, (x, y, pos) =>
            {
                GameObject nodeObject = UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity, parent);
                nodeObject.name = $"{x}, {y}";
                GridNode node = new GridNode(x, y, nodeObject, teamIndex);
                coordDictionary.Add(new Vector2Int(x, y), node);
                staticGridNodes[counter] = node;
                counter++;
            });

            callback();
            return staticGridNodes;
        }

        public static void GridScaffold(Vector3 center, Vector2Int gridSize, float spacing, Action<int, int, Vector3> method)
        {
            Vector3 createOffset = center - Vector3.right * gridSize.x / 2f - Vector3.forward * gridSize.y / 2f;
            
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 position = createOffset + Vector3.right * (x * spacing) + Vector3.forward * (y * spacing);
                    method(x, y, position);
                }
            }
        }

        public static GridNode GetNodeAt(int coordx, int coordy)
        {
            if (coordDictionary.TryGetValue(new Vector2Int(coordx, coordy), out GridNode node))
            {
                return node;
            }

            return null;
        }

        public static GridNode NodeFromWorldPoint(Vector3 pos)
        {
            float xPercent = (pos.x + staticGridSize.x / 2f) / staticGridSize.x;
            float yPercent = (pos.z + staticGridSize.y / 2f) / staticGridSize.y;
            xPercent = Mathf.Clamp01(xPercent);
            yPercent = Mathf.Clamp01(yPercent);
            int x = Mathf.RoundToInt((staticGridSize.x - 1) * xPercent);
            int z = Mathf.RoundToInt((staticGridSize.y - 1) * yPercent);

            return GetNodeAt(x,z);
        }
    }

    [System.Serializable]
    public class GridNode
    {
        private int coordx, coordy;
        private GameObject node;
        private GameObject tower;
        private int teamIndex;
        private Vector3 placementOffset = new Vector3(0, .1f, 0f);

        public Vector2Int Coords => new Vector2Int(coordx, coordy);
        public GameObject NodeObject => node;
        public GameObject TowerOnNode => tower;
        public bool IsOccupied => tower != null;
        public int TeamIndex => teamIndex;

        public GridNode(int coordx, int coordy, GameObject node, int teamIndex)
        {
            this.coordx = coordx;
            this.coordy = coordy;
            this.node = node;
            this.teamIndex = teamIndex;
        }

        public bool PlaceTower(GameObject towerAsset)
        {
            if (!IsOccupied)
            {
                tower = GameObject.Instantiate(towerAsset, node.transform.position + placementOffset, Quaternion.identity);
            }

            return !IsOccupied;
        }

        public void RemoveTower()
        {
            GameObject.Destroy(tower);
            tower = null;
        }
    }
}