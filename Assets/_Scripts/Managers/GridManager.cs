using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pathing;
using Game;
using Core.Assets;
using System.Threading.Tasks;

namespace Core.Grid
{
    public class GridManager : MonoBehaviour
    {   
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float gridSpacing = 1f;
        [SerializeField] private PathManager pathManager;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private int teamIndex = 0;

        private AssetManager assetManager;

        public GameObject HomeNode => pathManager.Path[pathManager.Path.Length - 1].TowerOnNode;

        public async Task GenerateGrid(Action<GridNode[]> beforePath, Action<GridNode[], GridNode[]> afterPath)
        {
            if (assetManager == null)
                assetManager = AssetManager.instance;

            GridNode[] grid = GridGenerator.CreateGrid(
                gridSize: gridSize, 
                spacing: gridSpacing, 
                center: transform.position, 
                prefab: assetManager.ThemeFile.Node, 
                teamIndex: teamIndex, 
                parent: transform 
            );

            Instantiate(assetManager.ThemeFile.Floor, Vector3.zero, Quaternion.identity);
            beforePath(grid);

            await pathManager.GeneratePath();
            pathManager.TokenSource.Cancel();
            pathManager.Path[0].ForcePlaceTower(assetManager.ThemeFile.Spawn);
            pathManager.Path[pathManager.Path.Length - 1].ForcePlaceTower(assetManager.ThemeFile.Home);

            afterPath(grid, pathManager.Path);
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