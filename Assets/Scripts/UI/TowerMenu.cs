using Game;
using Standalone;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu.UI
{
    public class TowerMenu : MonoBehaviour
    {
        [Header("Load Out")]
        [SerializeField] private int maxLoadOut = 5;
        [SerializeField] private Transform loadoutList;

        [Header("Tower List")]
        [SerializeField] private Transform towerList;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private TowerSO[] towers;

        [Header("Details")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI towerNameTMP;
        
        [SerializeField] private TextMeshProUGUI damageTMP;
        [SerializeField] private TextMeshProUGUI rangeTMP;
        [SerializeField] private TextMeshProUGUI fireRateTMP;
        [SerializeField] private TextMeshProUGUI projSpeedTMP;
        [SerializeField] private TextMeshProUGUI currencyTMP;
        [SerializeField] private TextMeshProUGUI upgradesTMP;
        [SerializeField] private TextMeshProUGUI piercingTMP;
        [SerializeField] private TextMeshProUGUI descTMP;

        private List<TowerSO> towerLoadOut;

        private void Start()
        {
            if (LoadOutManager.SelectedTowers.Count > 0)
                towerLoadOut = LoadOutManager.SelectedTowers;
            else
                towerLoadOut = new List<TowerSO>();

            DisplayTowers();
            UpdateLoadOut();
        }

        public void DisplayDetails(TowerSO towerSO)
        {
            TowerDetails towerDetails = towerSO.TreeList[0];
            icon.sprite = towerSO.BaseIcon;
            towerNameTMP.text = towerDetails.Name;

            Tower tower = towerSO.BaseTower.GetComponent<Tower>();
            damageTMP.text = tower.Damage.ToString();
            rangeTMP.text = tower.Range.ToString();
            fireRateTMP.text = tower.FireRate.ToString();
            projSpeedTMP.text = tower.ProjSpeed.ToString();
            currencyTMP.text = $"{towerSO.TowerCost}";
            upgradesTMP.text = $"{towerSO.TreeList.Length} Levels";
            piercingTMP.text = $"{tower.Piercing}%";
            descTMP.text = towerSO.TowerDesc;
        }

        public void DisplayTowers() 
        {
            for (int i = 0; i < towerList.childCount; i++)
            {
                Destroy(towerList.GetChild(i).gameObject);
            }

            TowerSO towerUpgradeTree;
            for (int i = 0; i < towers.Length; i++)
            {
                GameObject towerButton = Instantiate(buttonPrefab, towerList);
                towerButton.transform.GetChild(0).GetComponent<Image>().sprite = towers[i].BaseIcon;

                towerUpgradeTree = towers[i];

                TowerMenuButton towerMenuButton = towerButton.GetComponent<TowerMenuButton>();
                towerMenuButton.tower = towerUpgradeTree;
                towerMenuButton.towerMenu = this;
                AddListener(towerButton.GetComponent<Button>(), towerUpgradeTree, tower =>
                {
                    AddToLoadOut(tower);
                });
            }
        }

        private void AddListener(Button button, TowerSO tower, Action<TowerSO> method)
        {
            TowerSO towerSO = tower;
            button.onClick.AddListener(() =>
            {
                method(tower);
            });
        }

        public void UpdateLoadOut()
        {
            for (int i = 0; i < loadoutList.childCount; i++)
            {
                loadoutList.GetChild(i).GetChild(1).GetComponent<Image>().enabled = false;
            }

            for (int i = 0; i < towerLoadOut.Count; i++)
            {
                Image image = loadoutList.GetChild(i).GetChild(1).GetComponent<Image>();
                image.sprite = towerLoadOut[i].BaseIcon;
                image.enabled = true;

                Button button = loadoutList.GetChild(i).GetComponent<Button>();
                button.onClick.RemoveAllListeners();

                AddListener(button, towerLoadOut[i], tower =>
                {
                    RemoveFromLoadOut(tower);
                });
            }
        }

        public void AddToLoadOut(TowerSO towerSO)
        {
            if (towerLoadOut.Count < maxLoadOut)
            {
                if (!towerLoadOut.Contains(towerSO))
                {
                    towerLoadOut.Add(towerSO);
                }
            }

            LoadOutManager.SelectedTowers = towerLoadOut;
            UpdateLoadOut();
        }

        public void RemoveFromLoadOut(TowerSO tower)
        {
            if (towerLoadOut.Contains(tower))
            {
                towerLoadOut.Remove(tower);
            }

            LoadOutManager.SelectedTowers = towerLoadOut;
            UpdateLoadOut();
        }
    }
}