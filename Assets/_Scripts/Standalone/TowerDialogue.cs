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
        [SerializeField] private Image towerIcon;

        [SerializeField] private TextMeshProUGUI upgradeNameTMP;
        [SerializeField] private TextMeshProUGUI upgradeDescTMP;

        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;

        public Button UpgradeButton => upgradeButton;
        public Button SellButton => sellButton;

        public void SetDetails(TowerTreeObject towerDetails)
        {
            towerNameTMP.text = towerDetails.Name != "" ? towerDetails.Name : "Unnamed Tower";
            towerIcon.sprite = towerDetails.Icon != null ? towerDetails.Icon : null;

            upgradeNameTMP.text = towerDetails.UpgradeName != "" ? towerDetails.UpgradeName : "Unnamed Upgrade";
            upgradeDescTMP.text = towerDetails.UpgradeDesc != "" ? towerDetails.UpgradeDesc : "Empty upgrade description.";
        }

        public void UpdateDetails()
        {

        }
    }
}