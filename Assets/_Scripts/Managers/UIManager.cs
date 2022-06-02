using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using TMPro;
using External.CustomSlider;
using Towers;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI waveCounterTMP;
        [SerializeField] private CustomSlider baseSlider;
        [SerializeField] private Transform towerButtonHolder;

        private Button[] towerButtons;
        private LevelManager levelManager;
        private HomeTower homeTower;

        public Action OnUpdateUI;

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

            homeTower = levelManager.GridNodeManager.HomeNode.GetComponent<HomeTower>();
            baseSlider.Initialize(homeTower.CurrentHealth, homeTower.MaxHealth, false, true, false);
            homeTower.OnHealthModified += (health) =>
            {
                baseSlider.SetValue(health);
            };
        }

        private void UpdateWaveCounter()
        {
            waveCounterTMP.text = "Wave: " + (levelManager.WaveSpawner.WaveCount == levelManager.WaveSpawner.WavesSO.Waves.Length - 1 ?
            "The Final Wave" :
            (levelManager.WaveSpawner.WaveCount + 1).ToString());
        }

        private void UpdateTowerButtons()
        {
            for (int i = 0; i < towerButtons.Length; i++)
            {
                towerButtons[i].interactable = levelManager.CurrencyManager.TowerCosts.TowerCostList[i].Cost <= levelManager.CurrencyManager.CurrencyContainer.CurrentAmount;
            }
        }
    }
}
