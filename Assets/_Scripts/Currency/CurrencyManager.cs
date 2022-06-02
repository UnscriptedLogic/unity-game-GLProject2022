using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Core.Currency 
{
    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField] private CurrencySO currencyContainer;
        [SerializeField] private TowerCosts towerCosts;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI currencyTMP;

        public CurrencySO CurrencyContainer => currencyContainer;
        public TowerCosts TowerCosts => towerCosts;
        public Action OnCashModified;

        public void ModifyCurrency(ModificationType modificationType, float amount)
        {
            switch (modificationType)
            {
                case ModificationType.Add:
                    currencyContainer.CurrentAmount += amount;
                    break;
                case ModificationType.Subtract:
                    currencyContainer.CurrentAmount -= amount;
                    break;
                case ModificationType.Set:
                    currencyContainer.CurrentAmount = amount;
                    break;
                case ModificationType.Divide:
                    currencyContainer.CurrentAmount /= amount;
                    break;
                case ModificationType.Multiply:
                    currencyContainer.CurrentAmount *= amount;
                    break;
                default:
                    break;
            }

            OnCashModified?.Invoke();
            UpdateCashUI();
        }

        private void UpdateCashUI()
        {
            if (currencyTMP != null)
                currencyTMP.text = "$" + currencyContainer.CurrentAmount.ToString();
        }
    }       
}