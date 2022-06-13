﻿using System;
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
        public enum UIpages
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
        [SerializeField] private Transform towerButtonHolder;
        [SerializeField] private TowerDialogue towerDialogue;

        [SerializeField] private GameObject buildModeUI;
        [SerializeField] private GameObject gameModeUI;
        [SerializeField] private GameObject viewModeUI;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private GameObject lostUI;
        [SerializeField] private GameObject wonUI;

        private Button[] towerButtons;
        private LevelManager levelManager;
        private HomeTower homeTower;

        public TowerDialogue TowerDialogue => towerDialogue;
        public GameObject BuildModeUI => buildModeUI;
        public GameObject GameModeUI => gameModeUI;
        public GameObject ViewModeUI => viewModeUI;
        public GameObject LoadingUI => loadingUI;
        public GameObject LostUI => lostUI;
        public GameObject WonUI => wonUI;

        public void Initialize(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            levelManager.CurrencyManager.OnCashModified += UpdateTowerButtons;

            towerButtons = new Button[towerButtonHolder.childCount];
            for (int i = 0; i < towerButtonHolder.childCount; i++)
            {
                towerButtons[i] = towerButtonHolder.GetChild(i).GetComponent<Button>();
                towerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = levelManager.CurrencyManager.TowerCosts.TowerCostList[i].Cost.ToString() + " points";
            }

            levelManager.WaveSpawner.OnWaveStarted += UpdateWaveCounter;

            //homeTower = levelManager.GridNodeManager.HomeNode.GetComponent<HomeTower>();
            baseSlider.Initialize(homeTower.CurrentHealth, homeTower.MaxHealth, false, true, false);
            homeTower.OnHealthModified += (health) =>
            {
                baseSlider.SetValue(health);
            };
        }

        public void SetTowerDialogue(TowerTreeObject towerDetails, Tower tower, float currency)
        {
            towerDialogue.SetDetails(towerDetails);
            towerDialogue.UpdateStats(tower.Damage, tower.Range, tower.FireRate, tower.TurnSpeed, tower.ProjSpeed);
            towerDialogue.UpdateButtons(currency);
        }

        private void UpdateWaveCounter()
        {
            string waveText = "Wave: ";

            if (levelManager.WaveSpawner.WaveCount > levelManager.WaveSpawner.WavesSO.Waves.Length)
            {
                waveText += (levelManager.WaveSpawner.WaveCount + 1).ToString() + " [re-run]";
            } else if (levelManager.WaveSpawner.WaveCount == levelManager.WaveSpawner.WavesSO.Waves.Length - 1)
            {
                waveText = "The Final Wave!";
            } else
            {
                waveText += (levelManager.WaveSpawner.WaveCount + 1).ToString();
            }

            waveCounterTMP.text = waveText;
        }

        public void UpdateTowerButtons()
        {
            for (int i = 0; i < towerButtons.Length; i++)
            {
                towerButtons[i].interactable = levelManager.CurrencyManager.TowerCosts.TowerCostList[i].Cost <= levelManager.CurrencyManager.CurrencyContainer.CurrentAmount;
            }

            towerDialogue.UpdateButtons(levelManager.CurrencyManager.CurrencyContainer.CurrentAmount);
        }

        public void ShowOnlyUI(UIpages uiPages)
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

        public void ShowUI(UIpages uiPages, bool value = true)
        {
            switch (uiPages)
            {
                case UIpages.BuildMode:
                    buildModeUI.SetActive(value);
                    break;
                case UIpages.GameMode:
                    gameModeUI.SetActive(value);
                    break;
                case UIpages.ViewMode:
                    viewModeUI.SetActive(value);
                    break;
                case UIpages.Loading:
                    loadingUI.SetActive(value);
                    break;
                case UIpages.Lost:
                    lostUI.SetActive(value);
                    break;
                case UIpages.Won:
                    wonUI.SetActive(value);
                    break;
                default:
                    break;
            }
        }
    }
}
