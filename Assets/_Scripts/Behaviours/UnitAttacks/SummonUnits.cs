using Core.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class SummonUnits : UnitAbility
    {
        [Serializable]
        public class SummonSettings
        {
            [SerializeField] private GameObject unitToSummon;
            [SerializeField] private int amount;

            public int Amount => amount;
            public GameObject Unit => unitToSummon;
        }

        [SerializeField] private SummonSettings[] summons;
        [SerializeField] private float slowdownDuration;
        [SerializeField] private float slowPercentage;
        [SerializeField] private float spawnInterval;

        private float reduction;
        private float _duration;

        private float _interval;
        private int summonIndex;
        private int amountSpawned;

        bool slowed = false;

        public override void EnterState()
        {
            reduction = (context.Speed / 100) * slowPercentage;
            context.Speed -= reduction;
            summonIndex = 0;
            _duration = slowdownDuration;
            slowed = false;
        }

        private void SpawnEnemy(GameObject spawn, int waypoint)
        {
            //GameObject enemy = Instantiate(currSegment.enemyToSpawn, spawnLocation.position, Quaternion.identity, transform);
            PoolManager.instance.PullFromPool(spawn, item =>
            {
                item.transform.SetParent(context.LevelManager.WaveSpawner.transform);
                item.GetComponent<Unit>().InitializeEnemy(context.LevelManager, waypoint);
            });
        }


        public override void UpdateState()
        {
            if (_duration <= 0f && slowed == false)
            {
                context.Speed += reduction;
                slowed = true;
            } else
            {
                _duration -= Time.deltaTime;
            }

            if (_interval <= 0f)
            {
                SpawnEnemy(summons[summonIndex].Unit, context.WaypointIndex);
                amountSpawned++;

                if (amountSpawned >= summons[summonIndex].Amount)
                {
                    summonIndex++;
                    amountSpawned = 0;
                    if (summonIndex >= summons.Length - 1)
                    {
                        ExitState();
                    }
                }

                _interval = spawnInterval;
            } else
            {
                _interval -= Time.deltaTime;
            }
        }

        public override void ExitState()
        {
            context.ExitState();
        }
    }
}