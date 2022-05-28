using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pathing;

namespace Core.Grid
{
    public class GridManager : MonoBehaviour
    {   
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float gridSpacing = 1f;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject homePrefab;
        [SerializeField] private GameObject entitySpawnPrefab;

        [SerializeField] private bool drawGizmos;
        [SerializeField] private int teamIndex = 0;

        private PathManager pathManager;

        public void GenerateGrid(Action method)
        {
            GridGenerator.CreateGrid(
                gridSize: gridSize, 
                spacing: gridSpacing, 
                center: transform.position, 
                prefab: nodePrefab, 
                teamIndex: teamIndex, 
                parent: transform 
            );

            pathManager = PathManager.instance;
            pathManager.GeneratePath(() =>
            {
                pathManager.Path[0].ForcePlaceTower(entitySpawnPrefab);
                pathManager.Path[pathManager.Path.Length - 1].ForcePlaceTower(homePrefab);
                method();
            });
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