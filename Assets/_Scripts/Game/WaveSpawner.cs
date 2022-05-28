using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pooling;
using Units;

namespace Game.Spawning
{
    public class WaveSpawner : MonoBehaviour
    {
        [Serializable]
        public class WaveSegment
        {
            public GameObject enemyToSpawn;
            public int amount;
            public float interval;
            public float segmentInterval;

            [HideInInspector] public int _spawnAmount = 0;
            [HideInInspector] public float _spawnInterval = 0;

            public void ContinueSpawn()
            {
                _spawnInterval = interval;
                _spawnAmount++;
            }
        }

        [Serializable]
        public class Wave
        {
            public WaveSegment[] waveSegments;
            public float waveInterval = 5f;

            [HideInInspector] public float _waveInterval;
            [HideInInspector] public float _segmentInterval;
        }

        public Transform spawnLocation;
        public float startDelay = 5f;
        private float _startDelay;

        [SerializeField] private Transform cam;

        public Wave[] waves;
        private Wave currWave;
        private WaveSegment currSegment;

        private int waveIndex;
        private int segmentIndex;

        private bool stopSpawning = true;

        public void StartSpawner()
        {
            stopSpawning = false;
            _startDelay = startDelay;
            currWave = waves[waveIndex];
            currSegment = currWave.waveSegments[segmentIndex];
        }

        public void StopSpawner() => stopSpawning = true;
        public void ContinueSpawner() => stopSpawning = false;

        private void Update()
        {
            if (stopSpawning)
            {
                return;
            }

            if (_startDelay >= 0f)
            {
                _startDelay -= Time.deltaTime;
                return;
            }

            if (currWave._waveInterval <= 0f)
            {
                if (currWave._segmentInterval <= 0)
                {
                    SpawnSegment();
                }
                else
                {
                    currWave._segmentInterval -= Time.deltaTime;
                }
            }
            else
            {
                currWave._waveInterval -= Time.deltaTime;
            }
        }

        private void SpawnSegment()
        {
            if (currSegment._spawnInterval <= 0)
            {
                SpawnEnemy();

                if (currSegment._spawnAmount == currSegment.amount - 1)
                {
                    NextWaveSegment();
                }
                else
                {
                    currSegment.ContinueSpawn();
                }
            }
            else
            {
                currSegment._spawnInterval -= Time.deltaTime;
            }
        }

        private void NextWaveSegment()
        {
            currWave._segmentInterval = currWave.waveSegments[segmentIndex].segmentInterval;
            segmentIndex++;
            if (segmentIndex >= currWave.waveSegments.Length)
            {
                stopSpawning = true;
                return;
            }

            currSegment = currWave.waveSegments[segmentIndex];
        }   

        private void NextWave()
        {
            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                waveIndex = 0;
            }

            currWave = waves[waveIndex];
            currSegment = currWave.waveSegments[segmentIndex];
        }

        private void SpawnEnemy()
        {
            //GameObject enemy = Instantiate(currSegment.enemyToSpawn, spawnLocation.position, Quaternion.identity, transform);
            GameObject enemy = PoolManager.instance.PullFromPool(currSegment.enemyToSpawn);
            enemy.transform.SetParent(transform);
        }
    }

}