using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Backend;
using TMPro;
using Core;

namespace UI
{
    public class LeaderboardsController : MonoBehaviour
    {
        /*
         * Panel
         * Name
         * Value
         */

        [SerializeField] private GameObject lbScorePrefab;
        
        [Header("TowerCount")]
        [SerializeField] private Transform listParent;
        [SerializeField] private Button refreshButton;        
        [SerializeField] private Transform loadingPanel;
        [SerializeField] private Transform cannotFetchPanel;
        
        [Header("Highest Wave")]
        [SerializeField] private Transform hwListParent;
        [SerializeField] private Button hwRefreshButton;
        [SerializeField] private Transform hwLoadingPanel;
        [SerializeField] private Transform hwCannotFetchPanel;

        [Space(10)]
        [SerializeField] private Color myColor;

        private void Start()
        {
            refreshButton.onClick.AddListener(ShowScores);
        }

        public void ShowScores()
        {
            ShowTowerCountScores();
            ShowHighestWaveScores();
        }

        public void ShowTowerCountScores()
        {
            loadingPanel.gameObject.SetActive(true);
            refreshButton.interactable = false;

            for (int i = 0; i < listParent.childCount; i++)
            {
                Destroy(listParent.GetChild(i).gameObject);
            }

            PlayFabManager.GetTowerCountScores(success =>
            {
                foreach (var item in success.Leaderboard)
                {
                    GameObject scoreCard = Instantiate(lbScorePrefab, listParent);
                    scoreCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.DisplayName;
                    scoreCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.StatValue.ToString();

                    if (item.PlayFabId == GameManager.playfabID)
                    {
                        scoreCard.transform.GetChild(0).GetComponent<Image>().color = myColor;
                    }
                }

                cannotFetchPanel.gameObject.SetActive(false);
                loadingPanel.gameObject.SetActive(false);
                refreshButton.interactable = true;
            },
            error =>
            {
                cannotFetchPanel.gameObject.SetActive(true);
                refreshButton.interactable = true;
            });
        }

        public void ShowHighestWaveScores()
        {
            hwLoadingPanel.gameObject.SetActive(true);
            hwRefreshButton.interactable = false;

            for (int i = 0; i < hwListParent.childCount; i++)
            {
                Destroy(hwListParent.GetChild(i).gameObject);
            }

            PlayFabManager.GetWaveScores(success =>
            {
                foreach (var item in success.Leaderboard)
                {
                    GameObject scoreCard = Instantiate(lbScorePrefab, hwListParent);
                    scoreCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.DisplayName;
                    scoreCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.StatValue.ToString();

                    if (item.PlayFabId == GameManager.playfabID)
                    {
                        scoreCard.transform.GetChild(0).GetComponent<Image>().color = myColor;
                    }
                }

                hwCannotFetchPanel.gameObject.SetActive(false);
                hwLoadingPanel.gameObject.SetActive(false);
                hwRefreshButton.interactable = true;
            },
            error =>
            {
                hwCannotFetchPanel.gameObject.SetActive(true);
                hwRefreshButton.interactable = true;
            });
        }
    }
}