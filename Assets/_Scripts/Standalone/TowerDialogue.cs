using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI towerNameTMP;
    [SerializeField] private Image towerIcon;

    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDesc;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
}
