using UnityEngine;
using UnityEngine.InputSystem;
using UnscriptedEngine;

public class C_TDPlayerController : UController
{
    private GM_TowerDefenceGameMode levelManager;
    private P_TDPlayerPawn playerPawn;
    private InputActionMap defaultMap;
    private Vector2 mousePos;

    protected override void OnLevelStarted()
    {
        levelManager = GameMode.CastTo<GM_TowerDefenceGameMode>();

        defaultMap = levelManager.InputContext.FindActionMap("Default");
     
        base.OnLevelStarted();
    }

    protected override ULevelPawn PossessPawn()
    {
        playerPawn = levelManager.GetPlayerPawn().CastTo<P_TDPlayerPawn>();
        return playerPawn;
    }

    private void Update()
    {
        if (playerPawn == null) return;

        mousePos = defaultMap.FindAction("MousePosition").ReadValue<Vector2>();

        playerPawn.MovePlayerCamera(mousePos);
    }
}