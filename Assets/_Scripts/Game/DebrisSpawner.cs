using Core.Assets;
using Core.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Grid
{
    public static class DebrisSpawner
    {
        public static void GenerateDebri(GameObject[] debrisList, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject debri = MathHelper.RandomFromList(debrisList);
                GridNode node = GridGenerator.GetRandomEmptyNode();

                node.ForcePlaceTower(debri);
                node.TowerOnNode.transform.forward = MathHelper.OfVectorDirectionAroundY();
            }
        }
    } 
}
