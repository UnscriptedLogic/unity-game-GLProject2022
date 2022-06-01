using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using TMPro;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI waveCounterTMP;

        [SerializeField] private Transform towerButtonHolder;
        private Button[] towerButtons;

        private LevelManager levelManager;

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

            levelManager.WaveSpawner.OnWaveCompleted += () =>
            {
                waveCounterTMP.text = "Wave: " + (levelManager.WaveSpawner.WaveCount == levelManager.WaveSpawner.WavesSO.Waves.Length - 1 ? 
                "The Final Wave": 
                (levelManager.WaveSpawner.WaveCount + 1).ToString());
            };
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
