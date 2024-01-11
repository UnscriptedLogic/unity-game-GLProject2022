using Standalone;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class UIC_BuildHUD : UCanvasController
{
    [Header("Tower Load Out")]
    [SerializeField] private Transform towerList;
    [SerializeField] private TowerBtn towerBtnPrefab;

    public override void OnWidgetAttached(ULevelObject context)
    {
        CreateLoadoutTowerButtons();

        base.OnWidgetAttached(context);
    }

    private void CreateLoadoutTowerButtons()
    {
        List<TowerSO> selectedTowers = GameMode.GameInstance.CastTo<GI_CustomGameInstance>().SelectedTowers;
        for (int i = 0; i < selectedTowers.Count; i++)
        {
            Instantiate(towerBtnPrefab, towerList).Initalize(this, selectedTowers[i]);
        }
    }
}