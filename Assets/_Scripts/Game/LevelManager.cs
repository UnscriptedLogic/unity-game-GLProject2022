using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Building;
using Core.Currency;
using Core.UI;
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

    public enum GameState
    {
        None,
        Building,
        Viewing
    }

    public class LevelManager : MonoBehaviour
    {
        private LevelState levelState = LevelState.Start;
        private GameState gameState = GameState.None;

        [Header("Components")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private WaveIncome waveIncome;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private UIManager uiManager;

        private HomeTower homeTower;

        [Header("UI")]
        [SerializeField] private CustomSlider baseSlider;
        [SerializeField] private GameObject buildModeUI;
        [SerializeField] private GameObject gameModeUI;

        public LevelState CurrentLevelState => levelState;
        public GameState CurrentGameState => gameState;
        public CurrencyManager CurrencyManager => currencyManager;
        public WaveSpawner WaveSpawner => waveSpawner;

        private void Start()
        {
            EnterLevelState();
            EnterGameState();
        }

        private void Update()
        {
            UpdateLevelState();
            UpdateGameState();
        }

        private void EnterLevelState()
        {
            switch (levelState)
            {
                case LevelState.Start:
                    gridManager.GenerateGrid(() =>
                    {
                        SwitchLevelState(LevelState.Playing);
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

        private void EnterGameState()
        {
            switch (gameState)
            {
                case GameState.None:
                    buildModeUI.SetActive(false);
                    gameModeUI.SetActive(true);
                    break;
                case GameState.Building:
                    buildManager.EnableBuildMode();
                    buildModeUI.SetActive(true);
                    gameModeUI.SetActive(false);
                    break;
                case GameState.Viewing:
                    break;
                default:
                    break;
            }
        }

        private void UpdateLevelState()
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

        private void UpdateGameState()
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.Building:
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (buildManager.PlaceTower())
                        {
                            currencyManager.ModifyCurrency(ModificationType.Subtract, currencyManager.TowerCosts.GetTowerCost(buildManager.TowerToPlace));
                            SwitchGameState(GameState.None);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        SwitchGameState(GameState.None);
                    }
                    break;
                case GameState.Viewing:
                    break;
                default:
                    break;
            }
        }

        private void ExitLevelState()
        {
            switch (levelState)
            {
                case LevelState.Start:
                    currencyManager.ModifyCurrency(ModificationType.Set, currencyManager.CurrencyContainer.StartAmount);
                    uiManager.Initialize(this);
                    waveIncome.Initialize(this);
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

        private void ExitGameState()
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.Building:
                    buildManager.DisableBuildMode();
                    buildModeUI.SetActive(false);
                    gameModeUI.SetActive(true);
                    break;
                case GameState.Viewing:
                    break;
                default:
                    break;
            }
        }

        public void SwitchLevelState(LevelState newState)
        {
            ExitLevelState();
            levelState = newState;
            EnterLevelState();
        }

        public void SwitchGameState(GameState newState)
        {
            ExitGameState();
            gameState = newState;
            EnterGameState();
        }

        #region StateSetters
        public void SetGameLost() => SwitchLevelState(LevelState.Lost);
        public void SetGamePlaying() => SwitchLevelState(LevelState.Playing);
        public void SetGamePaused() => SwitchLevelState(LevelState.Paused);

        public void SetBuildMode() => SwitchGameState(GameState.Building);
        public void SetNoneMode() => SwitchGameState(GameState.None);
        #endregion
    }
}