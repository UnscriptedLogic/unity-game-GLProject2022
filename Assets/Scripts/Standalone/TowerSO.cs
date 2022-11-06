using System;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;

namespace Standalone
{
    [Serializable]
    public class TowerDetails
    {
        [SerializeField] private int towerID;
        [Space(10)]
        [SerializeField] private bool overrideTowerName;
        [SerializeField] private string towerName;
        [SerializeField] private Sprite towerIcon;
        [Space(10)]
        [SerializeField] private bool overrideSellCost;
        [SerializeField] private float sellCost;
        [Space(10)]
        [SerializeField] private Sprite upgradeIcon;
        [SerializeField] private float upgradeCost;
        [SerializeField] private string upgradeName;

        [Space(10)]
        [SerializeField] private bool overrideDesc;
        [TextArea(5, 10)]
        [SerializeField] private string upgradeDesc;
        [SerializeField] private GameObject nextUpgrade;

        public int ID => towerID;
        public bool OverrideTowerName => overrideTowerName;
        public string Name { get => towerName; set { towerName = value; } }
        public Sprite Icon => towerIcon;
        public bool OverrideSellCost => overrideSellCost;
        public float SellCost { get => sellCost; set { sellCost = value; } }
        public Sprite UpgradeIcon => upgradeIcon;
        public float UpgradeCost => upgradeCost;
        public string UpgradeName => upgradeName;

        public bool OverrideDesc => overrideDesc;
        public string UpgradeDesc { get => upgradeDesc; set { upgradeDesc = value; } }
        public GameObject UpgradedTower => nextUpgrade;
    }

    [CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/New Tower")]
    public class TowerSO : ScriptableObject
    {
        [SerializeField] private float towerCost;
        [SerializeField] private Vector2 requriedElevation;
        [SerializeField] private GameObject baseTower;
        [SerializeField] private Sprite baseIcon;

        [TextArea(5, 5)]
        [SerializeField] private string towerDescription;

        [SerializeField] private TowerDetails[] treeList;
        [SerializeField] private Vector2Int[] nodeRequirements;

        /**
         * Base node or node 0,0 is always at the top left.
         * (0,0)    (0,1)
         * (-1,0), (-1,1)
        **/
        [Tooltip("Leave Empty for 1x1 constructions")]
        public Vector2Int[] MultiNodeRequirements => nodeRequirements;
        public Vector2 RequiredElevation => requriedElevation;
        public float TowerCost => towerCost;
        public TowerDetails[] TreeList => treeList;
        public GameObject BaseTower => baseTower;
        public Sprite BaseIcon => baseIcon;
        public string TowerDesc => towerDescription;

        private void OnValidate()
        {
            treeList[0].SellCost = towerCost / 2;
            for (int i = 1; i < treeList.Length; i++)
            {
                if (!treeList[i].OverrideSellCost)
                    treeList[i].SellCost = treeList[i - 1].SellCost + treeList[i - 1].UpgradeCost / 2;

                if (!treeList[i].OverrideTowerName)
                    treeList[i].Name = $"Lvl {i + 1} {treeList[0].Name}";

                if (treeList[i].OverrideDesc == false)
                {
                    if (treeList[i].UpgradedTower != null)
                    {
                        Tower prevUpgrade = treeList[i - 1].UpgradedTower.GetComponent<Tower>();
                        Tower currentUpgrade = treeList[i].UpgradedTower.GetComponent<Tower>();
                        List<string> statIncrease = new List<string>();
                        List<string> statDecrease = new List<string>();

                        CompareStats("damage", currentUpgrade.Damage, prevUpgrade.Damage, statIncrease, statDecrease);
                        CompareStats("range", currentUpgrade.Range, prevUpgrade.Range, statIncrease, statDecrease);
                        CompareStats("fire rate", currentUpgrade.FireRate, prevUpgrade.FireRate, statIncrease, statDecrease, true);
                        CompareStats("projectile speed", currentUpgrade.ProjSpeed, prevUpgrade.ProjSpeed, statIncrease, statDecrease);
                        CompareStats("projectile lifetime", currentUpgrade.ProjLifeTime, prevUpgrade.ProjLifeTime, statIncrease, statDecrease);

                        string newDesc = "";
                        if (statIncrease.Count > 0)
                        {
                            newDesc += "Increases";
                            for (int j = 0; j < statIncrease.Count - 1; j++)
                            {
                                newDesc += $" {statIncrease[j]}";

                                if (j < statIncrease.Count - 2)
                                    newDesc += ", ";
                            }

                            if (statIncrease.Count > 1)
                                newDesc += $" and {statIncrease[statIncrease.Count - 1]}";
                            else
                                newDesc += $" {statIncrease[statIncrease.Count - 1]}";
                        }

                        if (statDecrease.Count > 0)
                        {
                            newDesc += "but decreases";
                            for (int j = 0; j < statDecrease.Count - 1; j++)
                            {
                                newDesc += $" {statDecrease[j]}";

                                if (j < statDecrease.Count - 2)
                                    newDesc += ", ";
                            }

                            if (statIncrease.Count > 1)
                                newDesc += $" and {statDecrease[statDecrease.Count - 1]}.";
                            else
                                newDesc += $" {statDecrease[statDecrease.Count - 1]}.";

                        } else
                        {
                            newDesc += ".";
                        }

                        treeList[i].UpgradeDesc = newDesc;
                    }
                }
            }
        }

        private void CompareStats(string statName, float value1, float value2, List<string> statIncrease, List<string> statDecrease, bool decreaseBetter = false)
        {
            if (decreaseBetter)
            {
                if (value1 < value2)
                    statIncrease.Add(statName);
                else if (value1 > value2)
                    statIncrease.Add(statName);
            }
            else 
            {
                if (value1 > value2)
                    statIncrease.Add(statName);
                else if (value1 > value2)
                    statIncrease.Add(statName);
            }
        }
    }
}