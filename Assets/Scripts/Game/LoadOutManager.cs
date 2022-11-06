using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Standalone;

namespace Game
{
    public static class LoadOutManager
    {
        [SerializeField] private static List<TowerSO> selectedTowers = new List<TowerSO>();

        public static List<TowerSO> SelectedTowers { get => selectedTowers; set { selectedTowers = value; } }
    }
}