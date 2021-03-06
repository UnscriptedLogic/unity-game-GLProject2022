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
using Core.Assets;
using Core;
using UnityEngine.Events;
using Backend;
using Interfaces;

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
        [SerializeField] private AssetManager assetManager;
        [SerializeField] private PlayerKeybinds playerKeybinds;
        [SerializeField] private PlayFabBehaviour playFabBehaviour;

        [SerializeField] private LayerMask UILayer;

        [Space(10)]
        [SerializeField] private float returnHomedelay = 5f;
        [SerializeField] private Vector2 elevationRange;
        [SerializeField] private Vector2 secondaryElevationRange;

        [Space(10)]
        [SerializeField] private TowerSO[] backupTowers;

        private LevelState levelState = LevelState.Start;
        private GameState gameState = GameState.None;

        public LevelState CurrentLevelState => levelState;
        public GameState CurrentGameState => gameState;
        public CurrencyManager CurrencyManager => currencyManager;
        public WaveSpawner WaveSpawner => waveSpawner;
        public GridManager GridNodeManager => gridManager;
        public PathManager PathManager => pathManager;
        public AssetManager AssetManager => assetManager;
        public BuildManager BuildManager => buildManager;

        public Action<LevelState> OnLevelStateChanged;
        public Action<GameState> OnGameStateChanged;
        public UnityEvent<int> OnGameWon;
        public UnityEvent<int> OnGameLost;
        public UnityEvent OnGameStarted;

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

        private async void EnterLevelState()
        {
            switch (levelState)
            {
                case LevelState.None:
                    break;
                case LevelState.Start:
                    uiManager.ShowOnlyUI(UIManager.Pages.Loading);
                    
                    if (GameManager.themeSO != null)
                    {
                        assetManager.ThemeFile = GameManager.themeSO;
                    }
                    RenderSettings.skybox = assetManager.ThemeFile.SkyboxMat;

                    await gridManager.GenerateGrid((grid) =>
                    {
                        
                    },
                    (grid, path) =>
                    {
                        GridAddOns.GenerateElevations((GridGenerator.GridSize.x / 2 + GridGenerator.GridSize.y / 2) / 2, grid, elevationRange, secondaryElevationRange);
                        GridAddOns.GenerateDebri(GridGenerator.GridSize.x + GridGenerator.GridSize.y, assetManager.ThemeFile.DebriList, grid);
                        SwitchLevelState(LevelState.Playing);
                    });
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    PausedUI();
                    Time.timeScale = 0f;
                    playFabBehaviour.HighestWaveCount(waveSpawner.WaveCount);
                    break;
                case LevelState.Won:
                    OnGameWon?.Invoke(waveSpawner.WaveCount);
                    uiManager.ShowOnlyUI(UIManager.Pages.Won);
                    waveSpawner.StopSpawner();

                    playFabBehaviour.HighestWaveCount(waveSpawner.WaveCount);
                    playFabBehaviour.UpdateLeaderboardTowerCount(buildManager.BuildCount);

                    GameManager.wins++;
                    GameManager.gamesPlayed++;
                    PlayFabManager.SavePlayerData(successs => { Debug.Log("Data Saved!"); }, PlayFabManager.HandleError);
                    break;
                case LevelState.Lost:
                    OnGameLost?.Invoke(waveSpawner.WaveCount);
                    uiManager.ShowOnlyUI(UIManager.Pages.Lost);
                    waveSpawner.StopSpawner();

                    playFabBehaviour.HighestWaveCount(waveSpawner.WaveCount);

                    GameManager.loss++;
                    GameManager.gamesPlayed++;
                    PlayFabManager.SavePlayerData(successs => { Debug.Log("Data Saved!"); }, PlayFabManager.HandleError);
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
                    uiManager.ShowOnlyUI(UIManager.Pages.GameMode);
                    break;
                case GameState.Building:
                    buildManager.EnableBuildMode();
                    uiManager.ShowOnlyUI(UIManager.Pages.BuildMode);
                    break;
                case GameState.Viewing:
                    uiManager.ShowUI(UIManager.Pages.ViewMode);
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
                        GridNode baseNode = buildManager.InspectedTower.OwnedGridNodes[0];
                        Vector3 position = buildManager.InspectedTower.transform.position;
                        buildManager.InspectedTower.RemoveSelf();
                        baseNode.PlaceTower(buildManager.InspectedTowerDetails.UpgradedTower);
                        baseNode.TowerOnNode.transform.position = position;

                        if (baseNode.TowerOnNode.TryGetComponent(out IRequirePath requirePath))
                        {
                            requirePath.InitPath(pathManager.Path);
                        }

                        buildManager.InspectedTower = baseNode.TowerOnNode.GetComponent<Tower>();
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
                            if (levelState != LevelState.Won || levelState != LevelState.Lost)
                            {
                                SwitchGameState(GameState.Viewing); 
                            }
                        }
                    }
                    break;
                case GameState.Building:
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (buildManager.PlaceTower())
                        {
                            currencyManager.ModifyCurrency(ModificationType.Subtract, buildManager.TowerTree.GetTowerTree(buildManager.TowerToPlaceScript.ID).TowerCost);
                            Destroy(Instantiate(assetManager.PlacedParticle, buildManager.PrevPlacedTower.transform.position, Quaternion.identity), 5f);

                            if (BuildManager.PrevPlacedTower.TryGetComponent(out IRequirePath requirePath))
                                requirePath.InitPath(pathManager.Path);

                            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                            {
                                if (buildManager.TryInspectTower())
                                {
                                    SwitchGameState(GameState.Viewing);
                                }
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.X))
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

                    if (Input.GetKeyDown(KeyCode.X))
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

                    TowerSO[] towers = LoadOutManager.SelectedTowers.Count != 0 ? LoadOutManager.SelectedTowers.ToArray() : backupTowers;
                    uiManager.Initialize(this, towers);
                    
                    currencyManager.OnCashModified += uiManager.UpdateTowerButtons;
                    currencyManager.ModifyCurrency(ModificationType.Set, currencyManager.CurrencyContainer.StartAmount);

                    waveSpawner.OnWaveStarted += uiManager.UpdateWaveCounter;
                    waveSpawner.OnWaveStarted += (curr, total) => currencyManager.ModifyCurrency(ModificationType.Add, waveIncome.AddWaveIncome(curr));
                    waveSpawner.OnWaveStarted += (curr, total) => { if (curr + 1 > GameManager.highestWave) GameManager.highestWave = curr + 1; };
                    waveSpawner.Initialize(pathManager.Path);
                    waveSpawner.StartSpawner();

                    playerKeybinds.Initialize(uiManager.TowerDialogue.UpgradeButton, uiManager.TowerDialogue.SellButton, uiManager.TowerButtonHolder);
                    OnGameStateChanged += playerKeybinds.UpdateGameState;
                    OnLevelStateChanged += playerKeybinds.UpdateLevelState;

                    OnGameStarted?.Invoke();
                    gameSceneManager.HideLoading();
                    break;
                case LevelState.Playing:
                    break;
                case LevelState.Paused:
                    Time.timeScale = 1f;
                    ResumeUI();
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
                    uiManager.ShowOnlyUI(UIManager.Pages.GameMode);
                    break;
                case GameState.Viewing:
                    uiManager.ShowUI(UIManager.Pages.ViewMode, false);
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

            OnLevelStateChanged?.Invoke(levelState);
        }

        public void SwitchGameState(GameState newState)
        {
            ExitGameState();
            gameState = newState;
            EnterGameState();

            OnGameStateChanged?.Invoke(gameState);
        }

        private void PausedUI()
        {
            uiManager.ToggleAllUI(false);
        }

        private void ResumeUI()
        {
            uiManager.ShowOnlyUI(UIManager.Pages.GameMode);
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