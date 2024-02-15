using Standalone;
using System.Linq;
using Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnscriptedEngine;
using UnscriptedEngine.BuildHandlers;

public class P_TDPlayerPawn : URTSCamera, IBuilder<Tower, GameObject>
{
    public enum PlayerState
    {
        Normal,
        Building,
        Deleting
    }

    [Header("Extension")]
    [SerializeField] private Vector2 panningDetectionThickness = new Vector2(5, 5);
    [SerializeField] private LayerMask nodeLayer;

    [Header("Scrolling")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private Vector2 scrollBounds;

    private PlayerState currentState;
    private GameObject previewObject;
    private int towerIndex;
    private Vector3 hitPosition;
    private bool isOverUI;

    private BuildHandlerSimple<Tower, GameObject, P_TDPlayerPawn> buildHandler;

    public GameObject[] buildableContainers
    {
        get
        {
            GI_CustomGameInstance gameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();
            GameObject[] prefabs = new GameObject[gameInstance.SelectedTowers.Count];
            for (int i = 0; i < gameInstance.SelectedTowers.Count; i++)
            {
                prefabs[i] = gameInstance.SelectedTowers[i].BaseTower;
            }

            return prefabs;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        cam = GetComponentInChildren<Camera>();
        buildHandler = new BuildHandlerSimple<Tower, GameObject, P_TDPlayerPawn>(this, buildableContainers.ToList());
    }

    private void OnEnterState()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Building:
                break;
            case PlayerState.Deleting:
                break;
            default:
                break;
        }
    }

    private void OnUpdateState()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Building:
                if (previewObject == null) break;

                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, nodeLayer))
                {
                    hitPosition = hit.transform.position + (Vector3.up * 0.25f);
                    previewObject.transform.position = hitPosition;
                }
                
                break;
            case PlayerState.Deleting:
                break;
            default:
                break;
        }
    }

    private void OnExitState()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Building:
                Debug.Log("Hello");
                Destroy(previewObject);
                break;
            case PlayerState.Deleting:
                break;
            default:
                break;
        }
    }

    private void SwitchState(PlayerState newState)
    {
        OnExitState();
        currentState = newState;
        OnEnterState();
    }

    public void EnterBuildMode(int index, TowerSO towerSO)
    {
        SwitchState(PlayerState.Building);
     
        towerIndex = index;
        previewObject = Instantiate(towerSO.BaseTower);
    }

    public void ControllerLeftMouseDown()
    {
        if (isOverUI) return;

        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Building:
                buildHandler.Build(towerIndex, hitPosition, Quaternion.identity, OnConditionResult);
                break;
            case PlayerState.Deleting:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();

        OnUpdateState();
    }

    public void MovePlayerCamera(Vector2 mousePos)
    {
        if (mousePos.x >= Screen.width - panningDetectionThickness.x)
        {
            MoveCamera(Direction.Right);
        }

        if (mousePos.x <= panningDetectionThickness.x)
        {
            MoveCamera(Direction.Left);
        }

        if (mousePos.y >= Screen.height - panningDetectionThickness.y)
        {
            MoveCamera(Direction.Forward);
        }

        if (mousePos.y <= panningDetectionThickness.y)
        {
            MoveCamera(Direction.Backward);
        }
    }

    public void ZoomCamera(float value)
    {
        Vector3 prevPos = cameraTransform.position;
        cameraTransform.Translate(cameraTransform.forward * value * (scrollSpeed * 100f) * Time.deltaTime, Space.World);

        if (cameraTransform.position.y <= scrollBounds.x || cameraTransform.position.y >= scrollBounds.y)
        {
            cameraTransform.position = prevPos;
        }
    }

    public Tower WhenGetBuildable(GameObject buildableObject)
    {
        return buildableObject.GetComponent<Tower>();
    }

    public void WhenCreateBuildable(int index, Vector3 position, Quaternion rotation, GameObject buildableContainer)
    {
        GameObject tower = Instantiate(buildableContainer, position, rotation);
    }

    public void OnConditionResult(BuildResult buildResult)
    {
        if (!buildResult.Passed)
        {
            Debug.Log(buildResult.Description);
        }
    }
}