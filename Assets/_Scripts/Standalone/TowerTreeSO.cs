using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    [CreateAssetMenu(fileName = "Tower Tree", menuName = "ScriptableObjects/New Tower Tree")]
    public class TowerTreeSO : ScriptableObject
    {
        [SerializeField] private TowerSO[] towerSO;

        public TowerDetails GetTowerDetail(int id)
        {
            for (int i = 0; i < towerSO.Length; i++)
            {
                for (int j = 0; j < towerSO[i].TreeList.Length; j++)
                {
                    if (towerSO[i].TreeList[j].ID == id)
                    {
                        return towerSO[i].TreeList[j];
                    }
                }
            }

            return null;
        }

        public TowerSO GetTowerTree(int id)
        {
            for (int i = 0; i < towerSO.Length; i++)
            {
                for (int j = 0; j < towerSO[i].TreeList.Length; j++)
                {
                    if (towerSO[i].TreeList[j].ID == id)
                    {
                        return towerSO[i];
                    }
                }
            }

            return null;
        }
    }
}