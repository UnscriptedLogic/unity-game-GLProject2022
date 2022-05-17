using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Grid
{
    public class GridManager : MonoBehaviour
    {   
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float gridSpacing = 1f;
        [SerializeField] private GameObject nodePrefab;

        [SerializeField] private bool drawGizmos;
        [SerializeField] private int teamIndex = 0;

        private void Start() => GridGenerator.CreateGrid(gridSize: gridSize, spacing: gridSpacing, center: transform.position, prefab: nodePrefab,teamIndex: teamIndex, parent: transform);

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