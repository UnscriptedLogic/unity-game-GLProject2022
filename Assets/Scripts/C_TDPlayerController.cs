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

        defaultMap.FindAction("MouseScroll").performed += C_TDPlayerController_performed;
     
        base.OnLevelStarted();
    }

    private void C_TDPlayerController_performed(InputAction.CallbackContext obj)
    {
        playerPawn.ZoomCamera(obj.ReadValue<float>());
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

    protected override void OnDestroy()
    {
        defaultMap.FindAction("MouseScroll").performed -= C_TDPlayerController_performed;

        base.OnDestroy();
    }
}