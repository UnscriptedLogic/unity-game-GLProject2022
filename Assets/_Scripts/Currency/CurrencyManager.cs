using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Core.Currency 
{
    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField] private CurrencySO currencyContainer;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI currencyTMP;

        public CurrencySO CurrencyContainer => currencyContainer;

        public void Initialize()
        {
            currencyContainer.ResetToStartAmount();
        }
    }       
}