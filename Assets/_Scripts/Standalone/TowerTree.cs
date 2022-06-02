using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    [CreateAssetMenu(fileName = "Tower Tree", menuName = "ScriptableObjects/New Tower Tree")]
    public class TowerTree : ScriptableObject
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
        }

        [SerializeField] private TowerTreeObject[] towerTreeObjects;
    }
}