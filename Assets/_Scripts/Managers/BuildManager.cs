using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;
using Towers;
using Standalone;

namespace Core.Building
{
    public class BuildManager : MonoBehaviour
    {
        [SerializeField] private bool buildMode;
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private LayerMask towerLayer;
        [SerializeField] private TowerTreeSO towerTree;
        private GameObject towerPrefab;
        private Tower inspectedTower;
        private TowerTreeObject inspectedTowerDetails;

        public bool BuildMode => buildMode;
        public GameObject TowerToPlace => towerPrefab;
        public Tower InspectedTower => inspectedTower;
        public TowerTreeSO TowerTree => towerTree;
        public TowerTreeObject InspectedTowerDetails => inspectedTowerDetails;

        public bool PlaceTower()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
            {
                string[] str = hit.transform.name.Split(",");
                int.TryParse(str[0], out int x);
                int.TryParse(str[1], out int y);

                return GridGenerator.GetNodeAt(x, y).PlaceTower(towerPrefab);
            }

            return false;
        }

        public bool TryInspectTower()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, towerLayer))
            {
                inspectedTower = hit.transform.GetComponent<Tower>();
                inspectedTowerDetails = towerTree.GetTowerDetail(inspectedTower.ID);
                return true;
            }

            return false;
        }

        public void SetBuildObject(GameObject newObject)
        {
            towerPrefab = newObject;
        }

        public void EnableBuildMode() 
        {
            buildMode = true;
        }

        internal void DisableBuildMode()
        {
            buildMode = false;
        }
    }
}