using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Building;
using Game.Spawning;
using External.CustomSlider;
using Towers;

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

        [Header("Components")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private WaveSpawner waveSpawner;

        private HomeTower homeTower;

        [Header("UI")]
        [SerializeField] private CustomSlider baseSlider;

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
                        homeTower = gridManager.HomeNode.GetComponent<HomeTower>();
                        baseSlider.Initialize(homeTower.CurrentHealth, homeTower.MaxHealth, false, true, false);
                        homeTower.OnHealthModified += (health) =>
                        {
                            baseSlider.SetValue(health);
                        };
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
                    Debug.Log("You won the game");
                    buildManager.enabled = false;
                    waveSpawner.StopSpawner();
                    break;
                case LevelState.Lost:
                    Debug.Log("You Lost the game");
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

        public void SwitchState(LevelState newState)
        {
            ExitState();
            levelState = newState;
            EnterState();
        }

        #region StateSetters
        public void SetGameLost() => SwitchState(LevelState.Lost);
        public void SetGamePlaying() => SwitchState(LevelState.Playing);
        public void SetGamePaused() => SwitchState(LevelState.Paused);
        #endregion
    }
}