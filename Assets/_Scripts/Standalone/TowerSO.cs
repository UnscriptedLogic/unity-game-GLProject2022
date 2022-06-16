using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    [Serializable]
    public class TowerDetails
    {
        [SerializeField] private int towerID;
        [SerializeField] private string towerName;
        [SerializeField] private Sprite towerIcon;
        [SerializeField] private float sellCost;
        [Space(10)]
        [SerializeField] private Sprite upgradeIcon;
        [SerializeField] private float upgradeCost;
        [SerializeField] private string upgradeName;
        [TextArea(5, 10)]
        [SerializeField] private string upgradeDesc;
        [SerializeField] private GameObject nextUpgrade;

        public int ID => towerID;
        public string Name => towerName;
        public Sprite Icon => towerIcon;
        public float SellCost => sellCost;
        public Sprite UpgradeIcon => upgradeIcon;
        public float UpgradeCost => upgradeCost;
        public string UpgradeName => upgradeName;
        public string UpgradeDesc => upgradeDesc;
        public GameObject UpgradedTower => nextUpgrade;
    }

    [CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/New Tower")]
    public class TowerSO : ScriptableObject
    {
        [SerializeField] private float towerCost;
        [SerializeField] private GameObject baseTower;
        [SerializeField] private TowerDetails[] treeList;

        public float TowerCost => towerCost;
        public TowerDetails[] TreeList => treeList;
        public GameObject BaseTower => baseTower;
    }
}