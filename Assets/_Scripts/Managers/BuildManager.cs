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
        [SerializeField] private Transform rangeVFX;
        [SerializeField] private Transform blueprintVFX;
        private GameObject towerPrefab;
        private GameObject prevPlacedTower;
        private Tower inspectedTower;
        private TowerTreeObject inspectedTowerDetails;

        private Vector3 mousePos;

        public bool BuildMode => buildMode;
        public GameObject TowerToPlace => towerPrefab;
        public GameObject PrevPlacedTower => prevPlacedTower;
        public Tower InspectedTower { get => inspectedTower; set { inspectedTower = value; } }
        public TowerTreeSO TowerTree => towerTree;
        public TowerTreeObject InspectedTowerDetails { get => inspectedTowerDetails; set { inspectedTowerDetails = value; } }

        private void Update()
        {
            if (buildMode)
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
                {
                    string[] str = hit.transform.name.Split(",");
                    int.TryParse(str[0], out int x);
                    int.TryParse(str[1], out int y);

                    GridNode node = GridGenerator.GetNodeAt(x, y);
                    if (!node.IsOccupied && !node.isObstacle)
                    {
                        float range = towerPrefab.GetComponent<Tower>().Range * 2f;
                        rangeVFX.localScale = new Vector3(range, rangeVFX.localScale.y, range);

                        rangeVFX.position = node.TowerPosition;
                        blueprintVFX.position = node.TowerPosition;

                        mousePos = Input.mousePosition;
                    }
                }
            }
        }

        public bool PlaceTower()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
            {
                string[] str = hit.transform.name.Split(",");
                int.TryParse(str[0], out int x);
                int.TryParse(str[1], out int y);

                GridNode node = GridGenerator.GetNodeAt(x, y);
                bool value = node.PlaceTower(towerPrefab);
                if (value)
                    prevPlacedTower = node.TowerOnNode;
                return value;
            }

            return false;
        }

        public bool TryInspectTower()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, towerLayer))
            {
                inspectedTower = hit.transform.GetComponent<Tower>();
                inspectedTowerDetails = towerTree.GetTowerDetail(inspectedTower.ID);
                VisualizeRange();
                return true;
            }

            return false;
        }

        public void VisualizeRange()
        {
            rangeVFX.gameObject.SetActive(true);
            rangeVFX.localScale = new Vector3(inspectedTower.Range * 2f, rangeVFX.localScale.y, InspectedTower.Range * 2f);
            rangeVFX.position = inspectedTower.transform.position;
        }

        public void HideRange()
        {
            rangeVFX.gameObject.SetActive(false);
            rangeVFX.SetParent(null);
            rangeVFX.localPosition = Vector3.zero;
        }

        public void SetBuildObject(GameObject newObject)
        {
            towerPrefab = newObject;
        }

        public void EnableBuildMode() 
        {
            buildMode = true;
            rangeVFX.gameObject.SetActive(true);
            blueprintVFX.gameObject.SetActive(true);
        }

        internal void DisableBuildMode()
        {
            buildMode = false;
            rangeVFX.gameObject.SetActive(false);
            blueprintVFX.gameObject.SetActive(false);
        }
    }
}