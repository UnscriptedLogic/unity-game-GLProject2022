using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Grid;
using Towers;
using Standalone;
using Interfaces;

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

        [Header("Animation")]
        [SerializeField] private LeanTweenType easeType = LeanTweenType.easeOutQuart;
        private float animTime = 0.05f;
        private int buildCount = 0;

        //VFX
        private Renderer rangeRenderer;
        private List<GameObject> extraBPs = new List<GameObject>();

        //Pre-building
        private TowerSO towerToPlaceSO;
        private Vector2 requiredElevation;
        private GridNode[] multitowerNodes;
        private Vector3 recenteredPosition;

        //Built
        private Tower inspectedTower;
        private TowerDetails inspectedTowerDetails;
        private GameObject prevPlacedTower;

        public int BuildCount => buildCount;
        public bool BuildMode => buildMode;
        public Vector3 RecenteredPos => recenteredPosition;
        public Tower TowerToPlaceScript => towerToPlaceSO.BaseTower.GetComponent<Tower>();
        public Tower InspectedTower { get => inspectedTower; set { inspectedTower = value; } }
        public GameObject TowerToPlace => towerToPlaceSO.BaseTower;
        public GameObject PrevPlacedTower => prevPlacedTower;
        public TowerTreeSO TowerTree => towerTree;
        public TowerDetails InspectedTowerDetails { get => inspectedTowerDetails; set { inspectedTowerDetails = value; } }

        private void Start()
        {
            buildCount = 0;
            if (rangeVFX != null && blueprintVFX != null)
            {
                rangeRenderer = rangeVFX.transform.GetChild(0).GetComponent<Renderer>();
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

                    Vector3 adjustedCenter = new Vector3(node.TowerPosition.x, 0.2f, node.TowerPosition.z);
                    rangeVFX.LeanMove(adjustedCenter, animTime).setEase(easeType);
                    blueprintVFX.LeanMove(node.TowerPosition, animTime).setEase(easeType);

                    if (SatisfiesRequirements(node))
                        SetBlueprintColor(validPlacement);
                    else
                        SetBlueprintColor(invalidPlacement);
                }
            }
        }

        private bool SatisfiesRequirements(GridNode node)
        {
            if (node.isCompletelyEmpty && node.isWithinElevation(towerToPlaceSO.RequiredElevation))
            {
                if (towerToPlaceSO.MultiNodeRequirements.Length > 0)
                {
                    if (GridGenerator.AreNodesEmpty(baseNode: node, coords: towerToPlaceSO.MultiNodeRequirements, elevationClamp: towerToPlaceSO.RequiredElevation, out multitowerNodes))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
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

                if (SatisfiesRequirements(node))
                {
                    bool value = node.PlaceTower(towerToPlaceSO.BaseTower);
                    if (value)
                    {
                        prevPlacedTower = node.TowerOnNode;
                        buildCount++;

                        if (multitowerNodes != null && multitowerNodes.Length > 0)
                        {
                            for (int i = 0; i < multitowerNodes.Length; i++)
                            {
                                multitowerNodes[i].isObstacle = true;
                                prevPlacedTower.GetComponent<Tower>().OwnedGridNodes.Add(multitowerNodes[i]);
                                multitowerNodes = new GridNode[0];
                            }

                            prevPlacedTower.transform.position += new Vector3(recenteredPosition.x, 0f, recenteredPosition.z);
                        }
                    }
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
            Vector3 adjustedPosition = new Vector3(inspectedTower.transform.position.x, 0.2f, inspectedTower.transform.position.z);
            if (rangeVFX.gameObject.activeSelf)
            {
                rangeVFX.LeanMove(adjustedPosition, animTime).setEase(easeType);
            }
            else
            {
                rangeVFX.position = adjustedPosition;
                rangeRenderer.transform.localScale = new Vector3(0f, rangeRenderer.transform.localScale.y, 0f);
            }

            rangeRenderer.transform.LeanScale(new Vector3(inspectedTower.Range * 2f, rangeRenderer.transform.localScale.y, InspectedTower.Range * 2f), animTime).setEase(easeType);

            SetBlueprintColor(viewingColor);
            rangeVFX.gameObject.SetActive(true);
        }

        public void HideRange()
        {
            rangeVFX.gameObject.SetActive(false);
            rangeVFX.SetParent(null);
            //rangeVFX.localPosition = Vector3.zero;
        }

        public void SetBuildObject(TowerSO towerSO)
        {
            towerToPlaceSO = towerSO;
            requiredElevation = towerTree.GetTowerTree(TowerToPlaceScript.ID).RequiredElevation;

            float range = towerToPlaceSO.BaseTower.GetComponent<Tower>().Range * 2f;
            rangeRenderer.transform.localScale = new Vector3(range, rangeRenderer.transform.localScale.y, range);

            if (towerSO.MultiNodeRequirements != null && towerSO.MultiNodeRequirements.Length > 0)
            {
                float totalX = blueprintVFX.GetChild(0).localPosition.x;
                float totalY = blueprintVFX.GetChild(0).localPosition.z;
                for (int i = 0; i < towerSO.MultiNodeRequirements.Length; i++)
                {
                    extraBPs.Add(Instantiate(blueprintVFX.GetChild(0).gameObject, blueprintVFX));
                    extraBPs[i].transform.localPosition = blueprintVFX.GetChild(0).localPosition + new Vector3(towerToPlaceSO.MultiNodeRequirements[i].x, 0f, towerToPlaceSO.MultiNodeRequirements[i].y);

                    totalX += extraBPs[i].transform.localPosition.x;
                    totalY += extraBPs[i].transform.localPosition.z;
                }

                float centerX = totalX / (towerToPlaceSO.MultiNodeRequirements.Length + 1) + 0.2f;
                float centerY = totalY / (towerToPlaceSO.MultiNodeRequirements.Length + 1) - 0.2f;

                recenteredPosition = new Vector3(centerX, 0f, centerY);

                rangeVFX.GetChild(0).localPosition = new Vector3(centerX, 0f, centerY);
            }
        }

        public void EnableBuildMode() 
        {
            buildMode = true;
            rangeVFX.gameObject.SetActive(true);
            blueprintVFX.gameObject.SetActive(true);
        }

        public void DisableBuildMode()
        {
            buildMode = false;
            rangeVFX.gameObject.SetActive(false);
            blueprintVFX.gameObject.SetActive(false);

            for (int i = 0; i < extraBPs.Count; i++)
            {
                Destroy(extraBPs[i]);
            }
            extraBPs.Clear();

            rangeRenderer.transform.localPosition = Vector3.zero;
        }
    }
}