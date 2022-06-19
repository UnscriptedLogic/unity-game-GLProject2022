using Core.Assets;
using Core.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GridAddOns
    {
        public static void GenerateDebri(int amount, GameObject[] debrisList, GridNode[] grid)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject debri = MathHelper.RandomFromArray(debrisList);
                GridNode node = GridGenerator.GetRandomEmptyNodeFrom(grid);

                node.ForcePlaceTower(debri);
                node.TowerOnNode.transform.forward = MathHelper.RandomVectorDirectionAroundY();
            }
        }

        public static void GenerateElevations(int elevations, GridNode[] grid, Vector2 primary, Vector2 secondary)
        {
            GridNode[] neighbours = new GridNode[8];
            for (int i = 0; i < elevations; i++)
            {
                GridNode node = GridGenerator.GetRandomEmptyNodeFrom(grid);
                float elevation = UnityEngine.Random.Range(primary.x, primary.y);
                node.NodeObject.transform.position = node.Position + Vector3.up * elevation; 


                neighbours[0] = GridGenerator.GetNodeAt(node.Coords.x - 1, node.Coords.y);
                neighbours[1] = GridGenerator.GetNodeAt(node.Coords.x + 1, node.Coords.y);
                neighbours[2] = GridGenerator.GetNodeAt(node.Coords.x, node.Coords.y + 1);
                neighbours[3] = GridGenerator.GetNodeAt(node.Coords.x, node.Coords.y - 1);
                neighbours[4] = GridGenerator.GetNodeAt(node.Coords.x - 1, node.Coords.y - 1);
                neighbours[5] = GridGenerator.GetNodeAt(node.Coords.x + 1, node.Coords.y + 1);
                neighbours[6] = GridGenerator.GetNodeAt(node.Coords.x - 1, node.Coords.y + 1);
                neighbours[7] = GridGenerator.GetNodeAt(node.Coords.x + 1, node.Coords.y - 1);

                for (int j = 0; j < neighbours.Length; j++)
                {
                    if (neighbours[j] != null)
                        if (!neighbours[j].isObstacle && !neighbours[j].IsOccupied)
                            neighbours[j].NodeObject.transform.position = neighbours[j].Position + Vector3.up * elevation;
                }
            }
        }

        private static void RandomizeHeight(GridNode node, Vector2 range)
        {
            if (node != null)
                if (!node.isObstacle && !node.IsOccupied)
                    node.NodeObject.transform.position = node.Position + Vector3.up * UnityEngine.Random.Range(range.x, range.y);
        }

        private static void RandomizeHeight(GridNode node, Vector2 range, out float elevation)
        {
            if (node != null) 
            {
                if (!node.isObstacle && !node.IsOccupied)
                {
                    elevation = UnityEngine.Random.Range(range.x, range.y);
                    node.NodeObject.transform.position = node.Position + Vector3.up * elevation;
                }    
            }

            elevation = 0f;
        }
    } 
}
