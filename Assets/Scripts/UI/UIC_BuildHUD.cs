using Standalone;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class OnBuildRequestedeventArgs : EventArgs
{
    public TowerSO towerSO;
    public int index;
}

public class UIC_BuildHUD : UCanvasController
{
    [Header("Tower Load Out")]
    [SerializeField] private Transform towerList;
    [SerializeField] private TowerBtn towerBtnPrefab;

    public event EventHandler<OnBuildRequestedeventArgs> OnBuildRequested;

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
            int index = i;
            
            TowerBtn button = Instantiate(towerBtnPrefab, towerList);
            button.Initalize(this, selectedTowers[index]);
            button.Bind(() => OnBuildRequested?.Invoke(this, new OnBuildRequestedeventArgs()
            {
                index = index,
                towerSO = selectedTowers[index]
            }));
        }
    }
}