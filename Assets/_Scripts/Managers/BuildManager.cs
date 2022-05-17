using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;

namespace Core.Building
{
    public class BuildManager : MonoBehaviour
    {
        [SerializeField] private bool buildMode;
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private GameObject towerPrefab;

        public bool BuildMode => buildMode;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
        }

        private void PlaceTower()
        {
            if (buildMode)
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
                {
                    string[] str = hit.transform.name.Split(",");
                    int.TryParse(str[0], out int x);
                    int.TryParse(str[1], out int y);
                    GridGenerator.GetNodeAt(x, y).PlaceTower(towerPrefab);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(cam.ScreenPointToRay(Input.mousePosition).origin, cam.ScreenPointToRay(Input.mousePosition).direction * 120f);
        }
    }
}