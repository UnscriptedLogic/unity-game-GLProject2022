using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
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
            }
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
