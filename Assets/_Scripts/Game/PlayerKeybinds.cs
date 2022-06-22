using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Standalone
{
    public class PlayerKeybinds : MonoBehaviour
    {
        [SerializeField] private KeyCode firstTowerSlot = KeyCode.Alpha1;
        [SerializeField] private KeyCode secondTowerSlot = KeyCode.Alpha2;
        [SerializeField] private KeyCode thirdTowerSlot = KeyCode.Alpha3;
        [SerializeField] private KeyCode fourthTowerSlot = KeyCode.Alpha4;
        [SerializeField] private KeyCode fifthTowerSlot = KeyCode.Alpha5;

        [Space(10)]
        [SerializeField] private KeyCode upgradeTower = KeyCode.Q;
        [SerializeField] private KeyCode sellTower = KeyCode.E;

        [Space(10)]
        [SerializeField] private KeyCode pauseKeybind = KeyCode.P;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;

        private Button upgradeButton;
        private Button sellButton;
        private List<Button> towerButtons = new List<Button>();

        private GameState gameState;
        private LevelState levelState;

        public void Initialize(Button upgradeButton, Button sellButton, Transform towerHolder)
        {
            this.upgradeButton = upgradeButton;
            this.sellButton = sellButton;

            for (int i = 0; i < towerHolder.childCount; i++)
            {
                towerButtons.Add(towerHolder.GetChild(i).GetComponent<Button>());
            }
        }

        private void Update()
        {
            if (gameState == GameState.Viewing)
            {
                if (Input.GetKeyDown(upgradeTower) && upgradeButton.IsInteractable())
                    upgradeButton.onClick.Invoke();

                if (Input.GetKeyDown(sellTower) && sellButton.IsInteractable())
                    sellButton.onClick.Invoke();
            }

            if (gameState == GameState.None || gameState == GameState.Viewing)
            {
                if (Input.GetKeyDown(firstTowerSlot))
                {
                    InvokeTowerButton(0);
                }

                if (Input.GetKeyDown(secondTowerSlot))
                {
                    InvokeTowerButton(1);
                }

                if (Input.GetKeyDown(thirdTowerSlot))
                {
                    InvokeTowerButton(2);
                }

                if (Input.GetKeyDown(fourthTowerSlot))
                {
                    InvokeTowerButton(3);
                }

                if (Input.GetKeyDown(fifthTowerSlot))
                {
                    InvokeTowerButton(4);
                }
            }

            if (Input.GetKeyDown(pauseKeybind))
            {
                if (levelState == LevelState.Paused)
                {
                    resumeButton.onClick.Invoke();
                }
                else
                {
                    pauseButton.onClick.Invoke();
                }
            }
        }

        private void InvokeTowerButton(int index)
        {
            if (index < towerButtons.Count)
            {
                if (towerButtons[index].IsInteractable())
                {
                    towerButtons[index].onClick.Invoke();
                } 
            }
        }

        public void UpdateGameState(GameState newState) => gameState = newState;
        public void UpdateLevelState(LevelState newState) => levelState = newState;
    }
}