using Interfaces;
using Standalone;
using Towers;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_InspectWindow : UCanvasController
{
    [SerializeField] private Image icon;
    private UTextComponent objectName;

    private GI_CustomGameInstance gameInstance;
    
    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);
        
        objectName = GetUIComponent<UTextComponent>("name");
        
        gameInstance = GameMode.GetGameInstance<GI_CustomGameInstance>();
    }

    public void SetInspectable(IInspectable inspectable)
    {
        if (inspectable is Tower tower)
        {
            InspectTower(tower);
        }
    }

    private void InspectTower(Tower tower)
    {
        TowerDetails towerDetails = gameInstance.AllTowerListSO.GetTowerDetail(tower.ID);
        
        icon.sprite = towerDetails.Icon;
        objectName.TMP.text = towerDetails.Name;
    }
}