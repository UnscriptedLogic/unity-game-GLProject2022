using UnityEngine;
using UnityEngine.InputSystem;
using UnscriptedEngine;

public class C_TDPlayerController : UController
{
    [SerializeField] private UIC_GameHUD gameHUDPrefab;
    [SerializeField] private UIC_BuildHUD buildHUDPrefab;

    private GM_TowerDefenceGameMode levelManager;
    private P_TDPlayerPawn playerPawn;
    private InputActionMap defaultMap;
    private Vector2 mousePos;

    private UIC_GameHUD gameHUD;
    private UIC_BuildHUD buildHUD;

    protected override void OnLevelStarted()
    {
        levelManager = GameMode.CastTo<GM_TowerDefenceGameMode>();

        defaultMap = GetDefaultInputMap();

        defaultMap.FindAction("MouseScroll").performed += C_TDPlayerController_performed;

        gameHUD = AttachUIWidget(gameHUDPrefab);
        buildHUD = AttachUIWidget(buildHUDPrefab);
        buildHUD.OnBuildRequested += BuildHUD_OnBuildRequested;

        base.OnLevelStarted();
    }

    public override void OnDefaultLeftMouseDown()
    {
        base.OnDefaultLeftMouseDown();
        playerPawn.ControllerLeftMouseDown();
    }

    private void BuildHUD_OnBuildRequested(object sender, OnBuildRequestedeventArgs args)
    {
        playerPawn.EnterBuildMode(args.index, args.towerSO);
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

        buildHUD.OnBuildRequested -= BuildHUD_OnBuildRequested;

        DettachUIWidget(gameHUD);
        DettachUIWidget(buildHUD);

        base.OnDestroy();
    }
}