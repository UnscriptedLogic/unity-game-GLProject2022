using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pathing;
using Core.Assets;

namespace Core.Grid
{
    public class MapManager : MonoBehaviour
    {   
        [Header("Components")]
        [SerializeField] private AssetSO assetSO;

        [Header("Grid")]
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float gridSpacing = 1f;
        [SerializeField] private int teamIndex = 0;
        [SerializeField] private bool drawGizmos;

        [Header("Path")]
        [SerializeField] private int seed = 10; //The seed for randomization
        [SerializeField] private int weightPointCount = 5; //The amount of 'crucial' points the path needs to meet (for proper path spread across the map)
        [SerializeField] private float weightPointDistance = 5f; //The amount of distance between the weightpoints;
        [SerializeField] private bool randomizeSeed = false;
        [SerializeField] private bool allowOverlap = false;

        [Header("Debris")]
        [SerializeField] private int debriAmount = 3;

        public bool RandomizeSeed => randomizeSeed;
        public bool AllowOverlap => allowOverlap;
        public int Seed => seed;
        public int WPcount => weightPointCount;
        public float WPdist => weightPointDistance;

        public void RandomSeed()
        {
            if (randomizeSeed)
            {
                seed = UnityEngine.Random.Range(-1000000, 1000000);
                UnityEngine.Random.InitState(seed);
            }
        }

        public void GenerateMap(Transform parent, PathManager pathManager)
        {
            GridGenerator.CreateGrid(
                gridSize: gridSize, 
                spacing: gridSpacing, 
                center: transform.position, 
                prefab: assetSO.NodePrefab, 
                teamIndex: teamIndex, 
                parent: parent
            );

            pathManager.Initialize(this);
            pathManager.GeneratePath(() =>
            {
                pathManager.Path[0].ForcePlaceTower(assetSO.SpawnPrefab);
                pathManager.Path[pathManager.Path.Length - 1].ForcePlaceTower(assetSO.HomePrefab);

                DebrisSpawner.GenerateDebri(assetSO.DebrisList, debriAmount, parent);
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