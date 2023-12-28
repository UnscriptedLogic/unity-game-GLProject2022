using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_PauseMenu : UCanvasController
{
    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        GameMode.PauseGame();

        Bind<UButtonComponent>("Resume", OnResume);
    }

    private void OnResume()
    {
        DettachUIWidget(this);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        GameMode.ResumeGame();

        base.OnWidgetDetached(context);

        Destroy(gameObject);
    }
}