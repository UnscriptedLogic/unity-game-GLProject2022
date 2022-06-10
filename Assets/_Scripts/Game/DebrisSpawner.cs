using Core.Assets;
using Core.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DebrisSpawner : MonoBehaviour
    {
        [SerializeField] private int debriAmount = 3;

        private AssetManager assetManager;

        public void Initialize(LevelManager levelManager)
        {
            assetManager = levelManager.AssetManager;
            GenerateDebri();
        }

        private void GenerateDebri()
        {
            for (int i = 0; i < debriAmount; i++)
            {
                GameObject debri = MathHelper.RandomFromList(assetManager.DebrisList);
                GridNode node = GridGenerator.GetRandomEmptyNode();

                node.ForcePlaceTower(debri);
                node.TowerOnNode.transform.forward = MathHelper.OfVectorDirectionAroundY();
            }
        }
    } 
}
