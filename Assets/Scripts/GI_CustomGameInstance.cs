using Standalone;
using System.Collections.Generic;
using UnityEngine;

public class GI_CustomGameInstance : UGameInstance
{
    [SerializeField] private List<TowerSO> selectedTowers = new List<TowerSO>();

    public List<TowerSO> SelectedTowers => selectedTowers;

    public void SetSelectedTowers(List<TowerSO> towers)
    {
        selectedTowers.Clear();
        selectedTowers = new List<TowerSO>(towers);
    }
}
