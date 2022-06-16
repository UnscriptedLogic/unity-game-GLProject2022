using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Standalone;

namespace Game
{
    public static class LoadOutManager
    {
        [SerializeField] private static List<TowerSO> selectedTowers = new List<TowerSO>();

        public static List<TowerSO> SelectedTowers => selectedTowers;

        public static void AddToLoadOut(TowerSO tower)
        {
            selectedTowers.Add(tower);
        }

        public static void RemoveFromLoadOut(TowerSO tower)
        {
            selectedTowers.Remove(tower);
        }

        public static void RemoveLast()
        {
            selectedTowers.RemoveAt(selectedTowers.Count - 1);
        }
    }
}