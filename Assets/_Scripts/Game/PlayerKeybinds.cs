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

        [Space(10)]
        [SerializeField] private KeyCode upgradeTower = KeyCode.Q;
        [SerializeField] private KeyCode sellTower = KeyCode.E;

        private Button upgradeButton;
        private Button sellButton;

        private GameState gameState;

        public void Initialize(Button upgradeButton, Button sellButton)
        {
            this.upgradeButton = upgradeButton;
            this.sellButton = sellButton;
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
        }

        public void UpdateGameState(GameState newState) => gameState = newState;
    }
}