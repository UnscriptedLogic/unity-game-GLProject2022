using Standalone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenu.UI
{
    public class TowerMenuButton : MonoBehaviour, IPointerEnterHandler
    {
        public TowerSO tower;
        public TowerMenu towerMenu;

        public void OnPointerEnter(PointerEventData eventData)
        {
            towerMenu.DisplayDetails(tower);
        }
    }
}