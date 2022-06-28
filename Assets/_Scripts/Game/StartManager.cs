using Core;
using Core.Assets;
using Core.Grid;
using Core.Scene;
using Game;
using Game.Spawning;
using Standalone;
using System.Collections.Generic;
using Towers;
using UI;
using UnityEngine;
using TMPro;
using Backend;
using UnityEngine.UI;

namespace StartScreen
{
    public class StartManager : MonoBehaviour
    {
        [SerializeField] private GameSceneManager gameSceneManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TowerTreeSO towerTreeSO;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private ThanksUIController thanksUIController;
        [SerializeField] private MainScreenUIController mainMenuScreen;
        [SerializeField] private AccountUIController accountUIController;
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private TextMeshProUGUI deviceTMP;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private ThemeToggleButton themeToggle;
        [SerializeField] private LayerMask nodeLayer;

        [Space(10)]
        [SerializeField] private int numberOfTowers;

        private List<GameObject> towers = new List<GameObject>();

        private async void Start()
        {
            versionText.text = $"Version: {Application.version}";
            
            if (!PlayerPrefs.HasKey(PlayFabManager.DEVICE_ID))
            {
                System.Guid guid = System.Guid.NewGuid();
                PlayerPrefs.SetString(PlayFabManager.DEVICE_ID, guid.ToString());
            }
            else if (PlayerPrefs.GetString(PlayFabManager.DEVICE_ID) == "")
            {
                System.Guid guid = System.Guid.NewGuid();
                PlayerPrefs.SetString(PlayFabManager.DEVICE_ID, guid.ToString());
            }

            GameManager.deviceID = PlayerPrefs.GetString(PlayFabManager.DEVICE_ID);
            deviceTMP.text = GameManager.deviceID.ToString();

            await gridManager.GenerateGrid((gridNodes) => { }, (gridNodes, path) =>
            {
                MapLogic(gridNodes);

                waveSpawner.Initialize(path);
                waveSpawner.StartSpawner();

            });

            if (!GameManager.loggedIn)
            {
                PlayFabManager.AnonymousLogIn(result =>
                {
                    Debug.Log("Success!");
                    GameManager.loggedIn = true;
                    leaderboardButton.interactable = true;
                    PlayFabManager.GetPlayerData(res => { }, PlayFabManager.HandleError);
                    gameSceneManager.HideLoading();
                }, error =>
                {
                    Debug.Log($"Could not log in to PlayFab {error.GenerateErrorReport()}");
                    deviceTMP.text = error.GenerateErrorReport();
                    leaderboardButton.interactable = false;
                    gameSceneManager.HideLoading();
                });

            } else
            {
                leaderboardButton.interactable = true;
                gameSceneManager.HideLoading();
            }

            if (!GameManager.hasSeenThanksPage && GameManager.gamesPlayed > 0)
            {
                mainMenuScreen.HideMainScreen();
                thanksUIController.ShowDialogue();
                GameManager.hasSeenThanksPage = true;
                mainMenuScreen.AboutButton.gameObject.SetActive(true);
                PlayFabManager.SavePlayerData(res => { }, PlayFabManager.HandleError);
            }

        }

        private void Update()
        {
            if (!mainMenuScreen.AboutButton.gameObject.activeSelf)
            {
                if (GameManager.hasSeenThanksPage)
                {
                    mainMenuScreen.AboutButton.gameObject.SetActive(true);
                }
            }
        }

        private void MapLogic(GridNode[] gridNodes)
        {
            GridAddOns.GenerateDebri(30, AssetManager.instance.ThemeFile.DebriList, gridNodes);
            GridAddOns.GenerateElevations(7, gridNodes, new Vector2(3, 3), new Vector2(3, 3));

            int count = 0;
            while (count < numberOfTowers)
            {
                TowerSO towerSO = MathHelper.RandomFromArray(towerTreeSO.TowerSOs);
                GameObject towerToPlace = GetRandomTower(towerSO);
                Tower tower = towerToPlace.GetComponent<Tower>();

                GridNode gridNode = MathHelper.RandomFromArray(gridNodes);
                GridNode[] neighbours = GridGenerator.GetNeighboursOf(gridNode);

                if (!gridNode.isObstacle && !gridNode.IsOccupied)
                {
                    if (gridNode.Elevation >= towerSO.RequiredElevation.x && gridNode.Elevation <= towerSO.RequiredElevation.y)
                    {
                        for (int i = 0; i < neighbours.Length; i++)
                        {
                            if (neighbours[i].isObstacle && !gridNode.IsOccupied)
                            {
                                gridNode.PlaceTower(towerToPlace);
                                count++;
                                towers.Add(gridNode.TowerOnNode.gameObject);
                                break;
                            }
                            else
                            {
                                Collider[] colliders = Physics.OverlapSphere(gridNode.Position, tower.Range - 2, nodeLayer);
                                for (int j = 0; j < colliders.Length; j++)
                                {
                                    GridNode node = GridGenerator.ParseGameObjectToNode(colliders[j].name);
                                    if (node != null)
                                    {
                                        if (node.isObstacle)
                                        {
                                            gridNode.PlaceTower(towerToPlace);
                                            count++;
                                            towers.Add(gridNode.TowerOnNode.gameObject);
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        private GameObject GetRandomTower(TowerSO tower)
        {
            TowerDetails towerDetails = MathHelper.RandomFromArray(tower.TreeList);
            while (towerDetails.UpgradedTower == null)
            {
                towerDetails = MathHelper.RandomFromArray(tower.TreeList);
            }

            return towerDetails.UpgradedTower;
        }
    }
}