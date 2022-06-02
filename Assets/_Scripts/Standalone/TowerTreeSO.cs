using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    [Serializable]
    public class TowerTreeObject
    {
        [SerializeField] private int towerID;
        [SerializeField] private string towerName;
        [SerializeField] private Sprite towerIcon;
        [SerializeField] private string upgradeName;

        [TextArea(5, 10)]
        [SerializeField] private string upgradeDesc;
        [SerializeField] private GameObject nextUpgrade;

        public int ID => towerID;
        public string Name => towerName;
        public Sprite Icon => towerIcon;
        public string UpgradeName => upgradeName;
        public string UpgradeDesc => upgradeDesc;
        public GameObject UpgradedTower => nextUpgrade;
    }

    [CreateAssetMenu(fileName = "Tower Tree", menuName = "ScriptableObjects/New Tower Tree")]
    public class TowerTreeSO : ScriptableObject
    {
        [SerializeField] private TowerTreeObject[] towerTreeObjects;

        public TowerTreeObject GetTowerDetail(int id)
        {
            for (int i = 0; i < towerTreeObjects.Length; i++)
            {
                if (towerTreeObjects[i].ID == id)
                {
                    return towerTreeObjects[i];
                }
            }

            return null;
        }
    }
}