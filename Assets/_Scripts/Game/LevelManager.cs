using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Building;
using Game.Spawning;

namespace Game
{
    public enum LevelState
    {
        Start,
        Playing,
        Paused,
        Won,
        Lost
    }

    public class LevelManager : MonoBehaviour
    {
        private LevelState levelState = LevelState.Start;

        [SerializeField] private GridManager gridManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private WaveSpawner waveSpawner;

        public static LevelManager instance;
        private void Awake() => instance = this;

        private void Start()
        {
            EnterState();
        }

        private void Update()
        {
            UpdateState();
        }

        private void EnterState()
        {
            switch (levelState)
            {
                case LevelState.Start:
                    gridManager.GenerateGrid(() =>
                    {
                        SwitchState(LevelState.Playing);
                    });
                    break;
                case LevelState.Playing:
                    buildManager.enabled = true;
                    waveSpawner.StartSpawner();
                    break;
                case LevelState.Paused:
                    buildManager.enabled = false;
                    break;
                case LevelState.Won:
                    buildManager.enabled = false;
                    waveSpawner.StopSpawner();
                    break;
                case LevelState.Lost:
                    buildManager.enabled = false;
                    waveSpawner.StopSpawner();
                    break;
                default:
                    break;
            }
        }

        private void UpdateState()
        {
            switch (levelState)
            {
                case LevelState.Start:
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    break;
                case LevelState.Won:
                    break;
                case LevelState.Lost:
                    break;
                default:
                    break;
            }
        }

        private void ExitState()
        {
            switch (levelState)
            {
                case LevelState.Start:
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    break;
                case LevelState.Won:
                    break;
                case LevelState.Lost:
                    break;
                default:
                    break;
            }
        }

        private void SwitchState(LevelState newState)
        {
            ExitState();
            levelState = newState;
            EnterState();
        }
    }
}