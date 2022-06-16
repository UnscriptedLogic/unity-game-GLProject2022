using Core.Assets;
using Core.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class DebrisSpawner
    {
        public static void GenerateDebri(int amount, GameObject[] debrisList, GridNode[] grid)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject debri = MathHelper.RandomFromList(debrisList);
                GridNode node = GridGenerator.GetRandomEmptyNodeFrom(grid);

                node.ForcePlaceTower(debri);
                node.TowerOnNode.transform.forward = MathHelper.RandomVectorDirectionAroundY();
            }
        }
    } 
}
