using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pathing;
using Game;

namespace Core.Grid
{
    public class GridManager : MonoBehaviour
    {   
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float gridSpacing = 1f;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject homePrefab;
        [SerializeField] private GameObject entitySpawnPrefab;
        [SerializeField] private PathManager pathManager;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private int teamIndex = 0;

        public GameObject HomeNode => pathManager.Path[pathManager.Path.Length - 1].TowerOnNode;

        public async void GenerateGrid(Action beforePath, Action afterPath)
        {
            GridGenerator.CreateGrid(
                gridSize: gridSize, 
                spacing: gridSpacing, 
                center: transform.position, 
                prefab: nodePrefab, 
                teamIndex: teamIndex, 
                parent: transform 
            );

            beforePath();

            await pathManager.GeneratePath();
            pathManager.Path[0].ForcePlaceTower(entitySpawnPrefab);
            pathManager.Path[pathManager.Path.Length - 1].ForcePlaceTower(homePrefab);

            afterPath();
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            GridGenerator.GridScaffold(transform.position, gridSize, gridSpacing, (x, y, pos) =>
            {
                Gizmos.DrawWireCube(pos, new Vector3(1f, 0f, 1f));
            });
        }
    }
}