using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using TMPro;
using External.CustomSlider;
using Towers;
using Standalone;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        public enum Pages
        {
            BuildMode,
            GameMode,
            ViewMode,
            Loading,
            Lost,
            Won
        }

        [SerializeField] private TextMeshProUGUI waveCounterTMP;
        [SerializeField] private CustomSlider baseSlider;
        [SerializeField] private TowerDialogue towerDialogue;
        [SerializeField] private TextMeshProUGUI currencyTMP;

        [SerializeField] private Transform towerButtonHolder;
        [SerializeField] private GameObject towerButtonPrefab;

        [SerializeField] private GameObject buildModeUI;
        [SerializeField] private GameObject gameModeUI;
        [SerializeField] private GameObject viewModeUI;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private GameObject lostUI;
        [SerializeField] private GameObject wonUI;

        private Button[] buttons;
        private LevelManager levelManager;
        private HomeTower homeTower;
        private TowerSO[] towers;

        public Transform TowerButtonHolder => towerButtonHolder;
        public TowerDialogue TowerDialogue => towerDialogue;
        public GameObject BuildModeUI => buildModeUI;
        public GameObject GameModeUI => gameModeUI;
        public GameObject ViewModeUI => viewModeUI;
        public GameObject LoadingUI => loadingUI;
        public GameObject LostUI => lostUI;
        public GameObject WonUI => wonUI;

        public void Initialize(LevelManager levelManager, TowerSO[] towers)
        {
            this.levelManager = levelManager;
            this.towers = towers;

            SetTowerBuyButtons(towers);

            homeTower = levelManager.GridNodeManager.HomeNode.GetComponent<HomeTower>();
            baseSlider.Initialize(homeTower.CurrentHealth, homeTower.MaxHealth, false, true, false);
            homeTower.OnHealthModified += (health) =>
            {
                baseSlider.SetValue(health);
            };
        }

        public void SetTowerBuyButtons(TowerSO[] towerSOs)
        {
            for (int i = 0; i < towerButtonHolder.childCount; i++)
            {
                Destroy(towerButtonHolder.GetChild(i).gameObject);
            }

            buttons = new Button[towerSOs.Length];
            for (int i = 0; i < towerSOs.Length; i++)
            {
                #region Initialization Stuff
                GameObject newButton = Instantiate(towerButtonPrefab, towerButtonHolder);
                buttons[i] = newButton.transform.GetComponent<Button>();
                buttons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 130f);
                buttons[i].transform.GetChild(0).GetComponent<Image>().sprite = towerSOs[i].BaseIcon;
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = towerSOs[i].TowerCost.ToString() + " points";
                #endregion

                GameObject baseTower = towerSOs[i].BaseTower;
                buttons[i].onClick.AddListener(() =>
                {
                    levelManager.SetBuildMode();
                    levelManager.BuildManager.SetBuildObject(baseTower);
                });
            }
        }

        public void SetTowerDialogue(TowerDetails towerDetails, Tower tower, float currency)
        {
            towerDialogue.SetDetails(towerDetails);
            towerDialogue.UpdateStats(tower.Damage, tower.Range, tower.FireRate, tower.TurnSpeed, tower.ProjSpeed);
            towerDialogue.UpdateButtons(currency);
        }

        public void UpdateWaveCounter(int current, int total)
        {
            string waveText = "Wave: ";

            if (current == total + 1)
            {
                waveText = "The Final Wave!";
            } else
            {
                waveText += (levelManager.WaveSpawner.WaveCount + 1).ToString() + $" / {total + 1}";
            }

            waveCounterTMP.text = waveText;
        }

        public void UpdateTowerButtons(ModificationType modificationType, float amount, float currentAmount)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = towers[i].TowerCost <= currentAmount;
            }

            towerDialogue.UpdateButtons(currentAmount);

            if (currencyTMP != null)
                currencyTMP.text = "$" + currentAmount.ToString();
        }

        public void ShowOnlyUI(Pages uiPages)
        {
            ToggleAllUI(false);

            ShowUI(uiPages);
        }

        public void ToggleAllUI(bool value)
        {
            buildModeUI.SetActive(value);
            gameModeUI.SetActive(value);
            viewModeUI.SetActive(value);
            loadingUI.SetActive(value);
            lostUI.SetActive(value);
            wonUI.SetActive(value);
        }

        public void ShowUI(Pages uiPages, bool value = true)
        {
            switch (uiPages)
            {
                case Pages.BuildMode:
                    buildModeUI.SetActive(value);
                    break;
                case Pages.GameMode:
                    gameModeUI.SetActive(value);
                    break;
                case Pages.ViewMode:
                    viewModeUI.SetActive(value);
                    break;
                case Pages.Loading:
                    loadingUI.SetActive(value);
                    break;
                case Pages.Lost:
                    lostUI.SetActive(value);
                    break;
                case Pages.Won:
                    wonUI.SetActive(value);
                    break;
                default:
                    break;
            }
        }
    }
}
