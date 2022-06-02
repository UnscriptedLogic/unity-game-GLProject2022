using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Building;
using Core.Currency;
using Core.UI;
using Game.Spawning;
using Towers;
using Standalone;
using UnityEngine.EventSystems;

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
        [Header("Components")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private WaveIncome waveIncome;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private UIManager uiManager;

        [Header("UI")]
        [SerializeField] private LayerMask UILayer;
        [SerializeField] private GameObject buildModeUI;
        [SerializeField] private GameObject gameModeUI;
        [SerializeField] private GameObject viewModeUI;
        [SerializeField] private TowerDialogue towerDialogue;

        private LevelState levelState = LevelState.Start;
        private GameState gameState = GameState.None;

        public LevelState CurrentLevelState => levelState;
        public GameState CurrentGameState => gameState;
        public CurrencyManager CurrencyManager => currencyManager;
        public WaveSpawner WaveSpawner => waveSpawner;
        public GridManager GridNodeManager => gridManager;

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
                    viewModeUI.SetActive(false);
                    break;
                case GameState.Building:
                    buildManager.EnableBuildMode();
                    buildModeUI.SetActive(true);
                    gameModeUI.SetActive(false);
                    break;
                case GameState.Viewing:
                    viewModeUI.SetActive(true);
                    towerDialogue.SetDetails(buildManager.InspectedTowerDetails);
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
                    if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("Click");

                        if (buildManager.TryInspectTower())
                        {
                            SwitchGameState(GameState.Viewing);
                            Debug.Log("Viewing!");
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
                            towerDialogue.SetDetails(buildManager.InspectedTowerDetails);
                        }
                        else if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            SwitchGameState(GameState.None);
                        }
                    }

                    towerDialogue.UpdateDetails();

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
                    viewModeUI.SetActive(false);
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

        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return true;
            }
            return false;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
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