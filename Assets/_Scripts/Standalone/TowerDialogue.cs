using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Standalone
{
    public class TowerDialogue : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI towerNameTMP;
        [SerializeField] private TextMeshProUGUI towerStatsTMP;
        [SerializeField] private Image towerIcon;

        [SerializeField] private Image upgradeIcon;
        [SerializeField] private TextMeshProUGUI upgradeNameTMP;
        [SerializeField] private TextMeshProUGUI upgradeDescTMP;

        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;

        private TowerTreeObject towerDetails;

        public Button UpgradeButton => upgradeButton;
        public Button SellButton => sellButton;

        public void SetDetails(TowerTreeObject _towerDetails)
        {
            towerDetails = _towerDetails;

            towerNameTMP.text = towerDetails.Name != "" ? towerDetails.Name : "Unnamed Tower";
            towerIcon.sprite = towerDetails.Icon != null ? towerDetails.Icon : null;

            upgradeIcon.sprite = towerDetails.UpgradeIcon != null ? towerDetails.UpgradeIcon : null;
            upgradeNameTMP.text = towerDetails.UpgradeName != "" ? towerDetails.UpgradeName : "Unnamed Upgrade";
            upgradeDescTMP.text = towerDetails.UpgradeDesc != "" ? towerDetails.UpgradeDesc : "Empty upgrade description.";

            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade(${towerDetails.UpgradeCost})";
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Sell(${towerDetails.SellCost})";
        }

        public void UpdateStats(float damage, float range, float fireRate, float turnSpeed, float projSpeed)
        {
            towerStatsTMP.text = 
                $"Damage: {damage}\n" +
                $"Range: {range}\n" +
                $"Fire Rate: {fireRate}\n" +
                $"Turn Speed: {turnSpeed}\n" +
                $"Proj. Speed: {projSpeed}";
        }

        public void UpdateButtons(float amount)
        {
            if (towerDetails != null)
                upgradeButton.interactable = amount >= towerDetails.UpgradeCost;
        }
    }
}