using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Building;
using Core.Currency;
using Core.UI;
using Core.Pathing;
using Core.Scene;
using Game.Spawning;
using Towers;
using Standalone;
using UnityEngine.EventSystems;
using System;

namespace Game
{
    public enum LevelState
    {
        None,
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
        [Header("Components")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private WaveIncome waveIncome;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private PathManager pathManager;
        [SerializeField] private GameSceneManager gameSceneManager;

        [Header("UI")]
        [SerializeField] private LayerMask UILayer;
        [SerializeField] private GameObject buildModeUI;
        [SerializeField] private GameObject gameModeUI;
        [SerializeField] private GameObject viewModeUI;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private GameObject lostUI;
        [SerializeField] private GameObject wonUI;

        [Space(10)]
        [SerializeField] private float returnHomedelay = 5f;

        private LevelState levelState = LevelState.Start;
        private GameState gameState = GameState.None;

        public LevelState CurrentLevelState => levelState;
        public GameState CurrentGameState => gameState;
        public CurrencyManager CurrencyManager => currencyManager;
        public WaveSpawner WaveSpawner => waveSpawner;
        public GridManager GridNodeManager => gridManager;
        public PathManager PathManager => pathManager;

        public Action OnLevelStateChanged;
        public Action OnGameStateChanged;

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
                case LevelState.None:
                    break;
                case LevelState.Start:
                    loadingUI.SetActive(true);
                    lostUI.SetActive(false);
                    wonUI.SetActive(false);
                    gridManager.GenerateGrid(() =>
                    {
                        SwitchLevelState(LevelState.Playing);

                    });
                    waveSpawner.Initialize(this);
                    waveSpawner.OnSpawningCompleted += () =>
                    {
                        SwitchLevelState(LevelState.Won);
                        SwitchGameState(GameState.None);
                    };
                    break;
                case LevelState.Playing:
                    uiManager.UpdateTowerButtons();
                    buildManager.enabled = true;
                    waveSpawner.StartSpawner();
                    break;
                case LevelState.Paused:
                    buildManager.enabled = false;
                    break;
                case LevelState.Won:
                    wonUI.SetActive(true);
                    buildManager.enabled = false;
                    waveSpawner.StopSpawner();
                    break;
                case LevelState.Lost:
                    lostUI.SetActive(true);
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
                    viewModeUI.SetActive(false);
                    break;
                case GameState.Building:
                    buildManager.EnableBuildMode();
                    buildModeUI.SetActive(true);
                    gameModeUI.SetActive(false);
                    break;
                case GameState.Viewing:
                    viewModeUI.SetActive(true);
                    uiManager.SetTowerDialogue(buildManager.InspectedTowerDetails, buildManager.InspectedTower, currencyManager.CurrencyContainer.CurrentAmount);
                    uiManager.TowerDialogue.SellButton.onClick.AddListener(() => 
                    { 
                        currencyManager.ModifyCurrency(ModificationType.Add, buildManager.InspectedTowerDetails.SellCost);
                        buildManager.InspectedTower.RemoveSelf();

                        uiManager.TowerDialogue.UpgradeButton.onClick.RemoveAllListeners();
                        uiManager.TowerDialogue.SellButton.onClick.RemoveAllListeners();

                        SwitchGameState(GameState.None);
                    });

                    uiManager.TowerDialogue.UpgradeButton.onClick.AddListener(() =>
                    {
                        currencyManager.ModifyCurrency(ModificationType.Subtract, buildManager.InspectedTowerDetails.UpgradeCost);
                        GridNode node = buildManager.InspectedTower.GridNode;
                        buildManager.InspectedTower.RemoveSelf();
                        node.PlaceTower(buildManager.InspectedTowerDetails.UpgradedTower);

                        buildManager.InspectedTower = node.TowerOnNode.GetComponent<Tower>();
                        buildManager.InspectedTowerDetails = buildManager.TowerTree.GetTowerDetail(buildManager.InspectedTower.ID);
                        
                        uiManager.TowerDialogue.UpgradeButton.onClick.RemoveAllListeners();
                        uiManager.TowerDialogue.SellButton.onClick.RemoveAllListeners();

                        SwitchGameState(GameState.Viewing);
                    });

                    buildManager.VisualizeRange();
                    break;
                default:
                    break;
            }
        }

        private void UpdateLevelState()
        {
            switch (levelState)
            {
                case LevelState.None:
                    break;
                case LevelState.Start:
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    break;
                case LevelState.Won:
                    if (returnHomedelay <= 0f)
                    {
                        SwitchLevelState(LevelState.None);
                    } else
                    {
                        returnHomedelay -= Time.deltaTime;
                    }
                    break;
                case LevelState.Lost:
                    if (returnHomedelay <= 0f)
                    {
                        SwitchLevelState(LevelState.None);
                    }
                    else
                    {
                        returnHomedelay -= Time.deltaTime;
                    }
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
                    if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                    {
                        if (buildManager.TryInspectTower())
                        {
                            SwitchGameState(GameState.Viewing);
                        }
                    }
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!EventSystem.current.IsPointerOverGameObject() && buildManager.TryInspectTower())
                        {
                            uiManager.SetTowerDialogue(buildManager.InspectedTowerDetails, buildManager.InspectedTower, currencyManager.CurrencyContainer.CurrentAmount);
                        }
                        else if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            SwitchGameState(GameState.None);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        SwitchGameState(GameState.None);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ExitLevelState()
        {
            switch (levelState)
            {
                case LevelState.None:
                    break;
                case LevelState.Start:
                    currencyManager.ModifyCurrency(ModificationType.Set, currencyManager.CurrencyContainer.StartAmount);
                    uiManager.Initialize(this);
                    waveIncome.Initialize(this);
                    loadingUI.SetActive(false);
                    gameSceneManager.HideLoading();
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    break;
                case LevelState.Won:
                    gameSceneManager.ReturnHome();
                    break;
                case LevelState.Lost:
                    gameSceneManager.ReturnHome();
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
                    viewModeUI.SetActive(false);
                    buildManager.HideRange();
                    uiManager.TowerDialogue.UpgradeButton.onClick.RemoveAllListeners();
                    uiManager.TowerDialogue.SellButton.onClick.RemoveAllListeners();
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

            OnLevelStateChanged?.Invoke();
        }

        public void SwitchGameState(GameState newState)
        {
            ExitGameState();
            gameState = newState;
            EnterGameState();

            OnGameStateChanged?.Invoke();
        }

        

        #region StateSetters
        public void SetGameLost() => SwitchLevelState(LevelState.Lost);
        public void SetGameWon() => SwitchLevelState(LevelState.Won);
        public void SetGamePlaying() => SwitchLevelState(LevelState.Playing);
        public void SetGamePaused() => SwitchLevelState(LevelState.Paused);

        public void SetBuildMode() => SwitchGameState(GameState.Building);
        public void SetNoneMode() => SwitchGameState(GameState.None);
        #endregion
    }
}