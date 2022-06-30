using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Towers;
using UnityEngine.Events;

namespace Core.Grid
{
    public static class GridGenerator
    {
        private static Vector2Int staticGridSize;
        private static GridNode[] staticGridNodes;
        private static Dictionary<Vector2Int, GridNode> coordDictionary;

        public static Vector2Int GridSize { get => staticGridSize; }
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

        public static GridNode GetNodeAt(Vector2Int coords)
        {
            if (coordDictionary.TryGetValue(coords, out GridNode node))
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

        public static GridNode GetRandomNode()
        {
            int coordx = UnityEngine.Random.Range(0, staticGridSize.x);
            int coordy = UnityEngine.Random.Range(0, staticGridSize.y);
            return GetNodeAt(coordx, coordy);
        }

        public static GridNode GetRandomNodeFrom(GridNode[] gridNodes)
        {
            return MathHelper.RandomFromArray(gridNodes);
        }

        public static GridNode GetRandomEmptyNode()
        {
            GridNode gridNode = null;

            bool invalid = true;
            int maxTries = 50;
            while (invalid || maxTries > 0)
            {
                int coordx = UnityEngine.Random.Range(0, staticGridSize.x - 1);
                int coordy = UnityEngine.Random.Range(0, staticGridSize.y - 1);

                gridNode = GetNodeAt(coordx, coordy);
                invalid = gridNode.IsOccupied || gridNode.isObstacle;
                maxTries--;
            }
            return gridNode;
        }

        public static GridNode GetRandomEmptyNodeFrom(GridNode[] gridNodes)
        {
            GridNode gridNode = null;

            bool invalid = true;
            int maxTries = 50;
            while (invalid || maxTries > 0)
            {
                gridNode = MathHelper.RandomFromArray(gridNodes);
                invalid = gridNode.IsOccupied || gridNode.isObstacle;
                maxTries--;
            }
            return gridNode;
        }

        public static bool AreNodesEmpty(Vector2Int[] coords, Vector2 elevationClamp)
        {
            bool value = true;
            for (int i = 0; i < coords.Length; i++)
            {
                GridNode gridnode = GetNodeAt(coords[i]);
                if (!gridnode.isWithinElevation(elevationClamp))
                {
                    value = false;
                    break;
                }

                if (gridnode.IsOccupied && gridnode.isObstacle)
                {
                    value = false;
                    break;
                }
            }

            return value;
        }

        public static GridNode[] GetNeighboursOf(GridNode node)
        {
            List<GridNode> neighbours = new List<GridNode>();

            GridNode neighbourNode = GetNodeAt(node.Coords.x - 1, node.Coords.y);
            if (neighbourNode != null)
                neighbours.Add(neighbourNode);

            //AddIfNotNull(node.Coords.x - 1, node.Coords.y, ref neighbours);
            AddIfNotNull(node.Coords.x + 1, node.Coords.y, ref neighbours);
            AddIfNotNull(node.Coords.x, node.Coords.y + 1, ref neighbours);
            AddIfNotNull(node.Coords.x, node.Coords.y - 1, ref neighbours);

            AddIfNotNull(node.Coords.x - 1, node.Coords.y - 1, ref neighbours);
            AddIfNotNull(node.Coords.x + 1, node.Coords.y + 1, ref neighbours);
            AddIfNotNull(node.Coords.x - 1, node.Coords.y + 1, ref neighbours);
            AddIfNotNull(node.Coords.x + 1, node.Coords.y - 1, ref neighbours);

            return neighbours.ToArray();
        }

        public static GridNode ParseGameObjectToNode(string name)
        {
            string[] str = name.Split(",");
            int.TryParse(str[0], out int x);
            int.TryParse(str[1], out int y);

            return GetNodeAt(x, y);
        }

        private static void AddIfNotNull(int x, int y, ref List<GridNode> gridNodes)
        {
            if (GetNodeAt(x, y) != null)
            {
                gridNodes.Add(GetNodeAt(x, y));
            }
        }
    }

    [System.Serializable]
    public class GridNode
    {
        private int coordx, coordy;
        private GameObject node;
        private GameObject tower;
        private int teamIndex;
        private Vector3 placementOffset = new Vector3(0, .2f, 0f);

        public bool isObstacle = false;
        public float hCost;
        public float gCost = float.MaxValue;
        public GridNode cameFromNode;

        public bool IsOccupied => tower != null;
        public bool isCompletelyEmpty => !isObstacle && !IsOccupied;
        public int TeamIndex => teamIndex;
        public float fCost => hCost + gCost;
        public float Elevation => node.transform.position.y;
        public GameObject NodeObject => node;
        public GameObject TowerOnNode => tower;
        public Vector2Int Coords => new Vector2Int(coordx, coordy);
        public Vector3 Position => node.transform.position;
        public Vector3 TowerPosition => node.transform.position + placementOffset;

        public GridNode(int coordx, int coordy, GameObject node, int teamIndex)
        {
            this.coordx = coordx;
            this.coordy = coordy;
            this.node = node;
            this.teamIndex = teamIndex;
        }

        public bool PlaceTower(GameObject towerAsset)
        {
            bool successful = false;
            if (!IsOccupied && !isObstacle)
            {
                tower = UnityEngine.Object.Instantiate(towerAsset, node.transform.position + placementOffset, Quaternion.identity);
                tower.GetComponent<Tower>().OwnedGridNode = this;
                successful = true;
            }

            return successful;
        }

        public void RemoveTower()
        {
            GameObject.Destroy(tower);
            tower = null;
        }

        public bool isWithinElevation(Vector2 elevationClamp)
        {
            if (Elevation >= elevationClamp.x && Elevation <= elevationClamp.y)
            {
                return true;
            }

            return false;
        }

        public void ForcePlaceTower(GameObject towerAsset) => tower = GameObject.Instantiate(towerAsset, node.transform.position + placementOffset, Quaternion.identity);
        public void SetColor(Color color) => NodeObject.GetComponent<Renderer>().material.color = color;
        public void SetVisibility(bool value) => NodeObject.GetComponent<Renderer>().enabled = value;

    }
}