using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_GameHUD : UCanvasController
{
    [SerializeField] private UIC_PauseMenu pauseUIPrefab;

    private ULevelObject context;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        this.context = context;

        Bind<UButtonComponent>("Pause", OnPause);
    }

    private void OnPause()
    {
        context.AttachUIWidget(pauseUIPrefab);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        UnBind<UButtonComponent>("Pause", OnPause);

        base.OnWidgetDetached(context);
    }
}