using Core.Pooling;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class InstantSummonUnits : UnitAbility
    {
        [Serializable]
        public class SummonSettings
        {
            [SerializeField] private GameObject unitToSummon;
            [SerializeField] private int amount;

            public int Amount => amount;
            public GameObject Unit => unitToSummon;
        }

        [SerializeField] private SummonSettings[] summonSettings;

        private bool front;
        private int increment;
        private int sign;

        private int waypointIndex;
        private LevelManager levelManager;

        public override void Initialize(GreaterUnit _context)
        {
            waypointIndex = _context.WaypointIndex;
            levelManager = _context.LevelManager;
        }

        public override void EnterState()
        {
            for (int i = 0; i < summonSettings.Length; i++)
            {
                SpawnEnemy(summonSettings[i].Unit, waypointIndex);
                for (int j = 1; j < summonSettings[i].Amount; j++)
                {
                    if (front)
                    {
                        sign = 1;
                        front = false;
                    }
                    else
                    {
                        sign = -1;
                        front = true;
                    }
                    
                    SpawnEnemy(summonSettings[i].Unit, waypointIndex + (j * sign));
                }
            }
        }

        private void SpawnEnemy(GameObject spawn, int waypoint)
        {
            PoolManager.instance.PullFromPool(spawn, item =>
            {
                item.transform.SetParent(levelManager.WaveSpawner.transform);
                item.GetComponent<Unit>().InitializeEnemy(levelManager, waypoint);
            });
        }
    }
}