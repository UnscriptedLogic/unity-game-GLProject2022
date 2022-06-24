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

        public override void Initialize(GreaterUnit _context)
        {
            context = _context;
            waypointIndex = _context.WaypointIndex;
        }

        public override void EnterState()
        {
            int count = 0;
            for (int i = 0; i < summonSettings.Length; i++)
            {
                for (int j = 0; j < summonSettings[i].Amount; j++)
                {
                    SpawnEnemy(summonSettings[i].Unit, waypointIndex);
                    count++;
                }
            }
        }

        private void SpawnEnemy(GameObject spawn, int waypoint)
        {
            PoolManager.instance.PullFromPool(spawn, item =>
            {
                item.transform.SetParent(transform.parent);
                item.GetComponent<Unit>().InitializeEnemy(context.Path, waypoint);

                float angle = UnityEngine.Random.Range(0f, 360f);
                float radius = 0.5f;
                float x = radius * Mathf.Cos(angle);
                float z = radius * Mathf.Sin(angle);
                Vector3 position = new Vector3(transform.position.x + x, item.transform.position.y, transform.position.z + z);
                item.transform.position = position;
            });
        }
    }
}