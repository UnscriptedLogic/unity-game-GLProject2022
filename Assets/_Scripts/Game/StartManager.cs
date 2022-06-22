using Core.Assets;
using Core.Grid;
using Core.Scene;
using Game;
using Game.Spawning;
using Standalone;
using System;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;

namespace Start
{
    public class StartManager : MonoBehaviour
    {
        [SerializeField] private GameSceneManager gameSceneManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TowerTreeSO towerTreeSO;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private LayerMask nodeLayer;

        [Space(10)]
        [SerializeField] private int numberOfTowers;

        private List<GameObject> towers = new List<GameObject>();

        private void Start()
        {
            gridManager.GenerateGrid((gridNodes) => { }, (gridNodes, path) =>
            {
                MapLogic(gridNodes);

                waveSpawner.Initialize(path);
                waveSpawner.StartSpawner();

                gameSceneManager.HideLoading();
            });
        }

        private void MapLogic(GridNode[] gridNodes)
        {
            GridAddOns.GenerateDebri(30, AssetManager.instance.ThemeFile.DebriList, gridNodes);
            GridAddOns.GenerateElevations(7, gridNodes, new Vector2(3, 3), new Vector2(3, 3));

            int count = 0;
            while (count < numberOfTowers)
            {
                TowerSO towerSO = MathHelper.RandomFromArray(towerTreeSO.TowerSOs);
                GameObject towerToPlace = GetRandomTower(towerSO);
                Tower tower = towerToPlace.GetComponent<Tower>();

                GridNode gridNode = MathHelper.RandomFromArray(gridNodes);
                GridNode[] neighbours = GridGenerator.GetNeighboursOf(gridNode);

                if (!gridNode.isObstacle && !gridNode.IsOccupied)
                {
                    if (gridNode.Elevation >= towerSO.RequiredElevation.x && gridNode.Elevation <= towerSO.RequiredElevation.y)
                    {
                        for (int i = 0; i < neighbours.Length; i++)
                        {
                            if (neighbours[i].isObstacle && !gridNode.IsOccupied)
                            {
                                gridNode.PlaceTower(towerToPlace);
                                count++;
                                towers.Add(gridNode.TowerOnNode.gameObject);
                                break;
                            }
                            else
                            {
                                Collider[] colliders = Physics.OverlapSphere(gridNode.Position, tower.Range - 2, nodeLayer);
                                for (int j = 0; j < colliders.Length; j++)
                                {
                                    GridNode node = GridGenerator.ParseGameObjectToNode(colliders[j].name);
                                    if (node != null)
                                    {
                                        if (node.isObstacle)
                                        {
                                            gridNode.PlaceTower(towerToPlace);
                                            count++;
                                            towers.Add(gridNode.TowerOnNode.gameObject);
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        private GameObject GetRandomTower(TowerSO tower)
        {
            TowerDetails towerDetails = MathHelper.RandomFromArray(tower.TreeList);
            while (towerDetails.UpgradedTower == null)
            {
                towerDetails = MathHelper.RandomFromArray(tower.TreeList);
            }

            return towerDetails.UpgradedTower;
        }
    }
}