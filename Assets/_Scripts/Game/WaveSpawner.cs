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
        private enum SpawnerStates
        {
            Stopped,
            SpawningWave,
            SpawningSegment,
            Waiting,
            Preparation,
            FinalWait
        }

        [SerializeField] private WavesSO wavesSO;
        [SerializeField] private float startDelay = 5f;
        [SerializeField] private int waveIndex;
        [SerializeField] private Transform spawnLocation;
        [SerializeField] private Transform cam;

        private Wave currWave;
        private WaveSegment currSegment;
        private SpawnerStates currentState = SpawnerStates.Stopped;
        private LevelManager levelManager;

        private float _interval;
        private int _spawnAmount;

        private int segmentIndex;
        private int waveCount;
        private bool stopSpawning = true;

        public int WaveCount => waveCount;
        public WavesSO WavesSO => wavesSO;
        public Action OnWaveCompleted;
        public Action OnWaveStarted;
        public Action OnSpawningCompleted;

        public void Initialize(LevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

        public void StartSpawner()
        {
            stopSpawning = false;
            currWave = wavesSO.Waves[waveIndex];
            currSegment = currWave.waveSegments[segmentIndex];
            waveCount = 0;

            SwitchState(SpawnerStates.Preparation);
        }

        private void Update()
        {
            if (stopSpawning)
                return;

            UpdateState();
        }

        private void EnterState()
        {
            switch (currentState)
            {
                case SpawnerStates.Stopped:
                    break;
                case SpawnerStates.SpawningWave:
                    currWave = wavesSO.Waves[waveIndex];
                    OnWaveStarted?.Invoke();
                    break;
                case SpawnerStates.SpawningSegment:
                    currSegment = wavesSO.Waves[waveIndex].waveSegments[segmentIndex];
                    _spawnAmount = 0;
                    break;
                case SpawnerStates.Waiting:
                    _interval = currSegment.segmentInterval;
                    break;                
                case SpawnerStates.Preparation:
                    _interval = startDelay;
                    break;
                case SpawnerStates.FinalWait:
                    break;
                default:
                    break;
            }
        }

        private void UpdateState()
        {
            switch (currentState)
            {
                case SpawnerStates.Stopped:
                    break;
                case SpawnerStates.SpawningWave:
                    if (_interval <= 0f)
                    {
                        SwitchState(SpawnerStates.SpawningSegment);
                        break;
                    } else
                    {
                        _interval -= Time.deltaTime;
                    }
                    break;
                case SpawnerStates.SpawningSegment:
                    if (_spawnAmount >= currSegment.amount) 
                    {
                        SwitchState(SpawnerStates.Waiting);
                        break;
                    }

                    if (_interval <= 0f)
                    {
                        SpawnEnemy();
                        _spawnAmount++;
                        _interval = currSegment.interval;
                    } else
                    {
                        _interval -= Time.deltaTime;
                    }
                    break;
                case SpawnerStates.Waiting:
                    if (_interval <= 0f)
                    {
                        segmentIndex++;
                        if (segmentIndex >= wavesSO.Waves[waveIndex].waveSegments.Length)
                        {
                            segmentIndex = 0;
                            waveIndex++;
                            waveCount++;
                            OnWaveCompleted?.Invoke();
                            if (waveIndex >= wavesSO.Waves.Length)
                            {
                                SpawningCompleted();
                                break;
                            }

                            _interval = currWave.waveInterval;
                            SwitchState(SpawnerStates.SpawningWave);
                            break;
                        }

                        SwitchState(SpawnerStates.SpawningSegment);
                    } else {
                        _interval -= Time.deltaTime;
                    }
                    break;
                case SpawnerStates.Preparation:
                    if (_interval <= 0f)
                    {
                        SwitchState(SpawnerStates.SpawningWave);
                    } else
                    {
                        _interval -= Time.deltaTime;
                    }
                    break;
                case SpawnerStates.FinalWait:
                    if (transform.childCount <= 0)
                    {
                        levelManager.SetGameWon();
                        Debug.Log(transform.childCount, gameObject);

                    }
                    break;
                default:
                    break;
            }
        }

        private void ExitState()
        {
            switch (currentState)
            {
                case SpawnerStates.Stopped:
                    break;
                case SpawnerStates.SpawningWave:
                    break;
                case SpawnerStates.SpawningSegment:
                    break;
                case SpawnerStates.Waiting:
                    break;
                case SpawnerStates.Preparation:
                    break;
                case SpawnerStates.FinalWait:
                    break;
                default:
                    break;
            }
        }

        private void SpawningCompleted()
        {
            SwitchState(SpawnerStates.FinalWait);
        }

        private void SwitchState(SpawnerStates newState)
        {
            ExitState();
            currentState = newState;
            EnterState();
        }

        public void StopSpawner() => stopSpawning = true;
        public void ContinueSpawner() => stopSpawning = false;

        private void ResetSpawner()
        {
            waveIndex = 0;
            segmentIndex = 0;

            currWave = wavesSO.Waves[waveIndex];
            currSegment = currWave.waveSegments[segmentIndex];

            SwitchState(SpawnerStates.SpawningWave);
        }

        private void SpawnEnemy()
        {
            //GameObject enemy = Instantiate(currSegment.enemyToSpawn, spawnLocation.position, Quaternion.identity, transform);
            PoolManager.instance.PullFromPool(currSegment.enemyToSpawn, item =>
            {
                item.transform.SetParent(transform);
                item.GetComponent<UnitMovement>().InitializeEnemy(levelManager);
            });
        }

        public void ClearEntities()
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                transform.GetChild(0).GetComponent<UnitMovement>().DestroyUnit();
            }
        }
    }

}