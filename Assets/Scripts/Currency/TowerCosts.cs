using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Currency
{
    [CreateAssetMenu(fileName = "Tower Costs", menuName = "ScriptableObjects/Costs")]
    public class TowerCosts : ScriptableObject 
    {
        [Serializable]
        public class TowerCost
        {
            [SerializeField] private GameObject tower;
            [SerializeField] private int cost;

            public int Cost => cost;

            public bool isSameTower(GameObject subject)
            {
                return tower == subject;
            }
        }

        //[SerializeField] private TowerCost[] towerCosts;
        //public TowerCost[] TowerCosts => towerCosts;

        [SerializeField] private TowerCost[] towerCosts;
        public TowerCost[] TowerCostList => towerCosts;


        public int GetTowerCost(GameObject subject)
        {
            for (int i = 0; i < towerCosts.Length; i++)
            {
                if (towerCosts[i].isSameTower(subject))
                {
                    return towerCosts[i].Cost;
                }
            }

            return 0;
        }
    }
}
