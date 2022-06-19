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

        [SerializeField] private Color viewingColor;
        [SerializeField] private Color validPlacement;
        [SerializeField] private Color invalidPlacement;

        private Renderer rangeRenderer;
        private GameObject towerPrefab;
        private Vector2 requiredElevation;
        private GameObject prevPlacedTower;
        private Tower inspectedTower;
        private TowerDetails inspectedTowerDetails;

        public bool BuildMode => buildMode;
        public Tower TowerToPlaceScript => towerPrefab.GetComponent<Tower>();
        public Tower InspectedTower { get => inspectedTower; set { inspectedTower = value; } }
        public GameObject TowerToPlace => towerPrefab;
        public GameObject PrevPlacedTower => prevPlacedTower;
        public TowerTreeSO TowerTree => towerTree;
        public TowerDetails InspectedTowerDetails { get => inspectedTowerDetails; set { inspectedTowerDetails = value; } }

        private void Start()
        {
            if (rangeVFX != null && blueprintVFX != null)
            {
                rangeRenderer = rangeVFX.GetComponent<Renderer>();
            }
            else
                Debug.LogWarning("RangeVFX or BlueprintVFX not assigned!");
        }

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
                    rangeVFX.position = node.TowerPosition;
                    blueprintVFX.position = node.TowerPosition;

                    if (SatisfiesRequirements(node))
                        SetBlueprintColor(validPlacement);
                    else
                        SetBlueprintColor(invalidPlacement);
                }
            }
        }

        private bool SatisfiesRequirements(GridNode node)
        {
            bool valid = false;

            if (!node.IsOccupied && !node.isObstacle)
                if (node.Elevation >= requiredElevation.x && node.Elevation <= requiredElevation.y)
                    valid = true;
            return valid;
        }

        private void SetBlueprintColor(Color color)
        {
            rangeRenderer.material.color = color;
        }

        public bool PlaceTower()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
            {
                string[] str = hit.transform.name.Split(",");
                int.TryParse(str[0], out int x);
                int.TryParse(str[1], out int y);

                GridNode node = GridGenerator.GetNodeAt(x, y);

                if (node.Elevation >= requiredElevation.x && node.Elevation <= requiredElevation.y)
                {
                    bool value = node.PlaceTower(towerPrefab);
                    if (value)
                        prevPlacedTower = node.TowerOnNode;
                    return value;
                }
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
            SetBlueprintColor(viewingColor);
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
            requiredElevation = towerTree.GetTowerTree(TowerToPlaceScript.ID).RequiredElevation;

            float range = towerPrefab.GetComponent<Tower>().Range * 2f;
            rangeVFX.localScale = new Vector3(range, rangeVFX.localScale.y, range);
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