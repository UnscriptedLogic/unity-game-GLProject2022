using Standalone;
using System.Linq;
using Core.Pooling;
using Interfaces;
using Towers;
using Unity.Cinemachine;
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
    [SerializeField] private LayerMask debriLayer;

    [Header("Scrolling")] 
    [SerializeField] private CinemachineCameraOffset camOffset;
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private Vector2 scrollBounds;
    private Transform cameraTransform;

    [Header("Building")] 
    [SerializeField] private RangeVisualizer rangeVisualizer;
    [SerializeField] private Material[] buildingMaterials;

    [Header("Inspection")] 
    [SerializeField] private UIC_InspectWindow inspectWindowPrefab;
    
    private UIC_InspectWindow inspectWindow;

    [Header("Audio")] 
    [SerializeField] private AudioClip towerBuildSFX;
    
    private Material canPlaceMaterial => buildingMaterials[0];
    private Material cannotPlaceMaterial => buildingMaterials[1];

    private PlayerState currentState;
    private GameObject previewObject;
    private int towerIndex;
    private Vector3 hitPosition;
    private bool isOverUI;
    private Vector3 previousProposedPosition;

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

        cam = Camera.main;
        cameraTransform = cam.transform;
        buildHandler = new BuildHandlerSimple<Tower, GameObject, P_TDPlayerPawn>(this, buildableContainers.ToList());
    }

    private void OnEnterState()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Building:
                
                if (inspectWindow != null)
                {
                    DettachUIWidget(inspectWindow);
                }
                
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
                
                if (previousProposedPosition != hitPosition)
                {
                    previousProposedPosition = hitPosition;
                    buildHandler.AdminConditionCheck(previewObject.GetComponent<Tower>(), out BuildResult adminBuildResult);
                    buildHandler.LocalConditionCheck(previewObject.GetComponent<Tower>(), hitPosition, Quaternion.identity, out BuildResult localBuildResult);
                    
                    void SetMaterial(Material material)
                    {
                        MeshRenderer[] meshRenderers = previewObject.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < meshRenderers.Length; i++)
                        {
                            Material[] materials = meshRenderers[i].materials;
                            for (int j = 0; j < materials.Length; j++)
                            {
                                materials[j] = material;
                            }
                            
                            meshRenderers[i].materials = materials;
                        }
                    }
                    
                    if (adminBuildResult.Passed && localBuildResult.Passed)
                    {
                        SetMaterial(canPlaceMaterial);
                    }
                    else
                    {
                        SetMaterial(cannotPlaceMaterial);
                    }
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
        
        Tower tower = previewObject.GetComponent<Tower>();
        tower.enabled = false;

        RangeVisualizer visualizer = PoolManager.instance.PullFromPool(rangeVisualizer.gameObject).GetComponent<RangeVisualizer>();
        visualizer.transform.SetParent(previewObject.transform);
        visualizer.ShowRange(tower.Range);
    }

    public void ControllerLeftMouseDown()
    {
        if (isOverUI) return;

        switch (currentState)
        {
            case PlayerState.Normal:
                
                if (inspectWindow != null)
                {
                    DettachUIWidget(inspectWindow);
                }
                
                // Raycast to check if we hit an inspectable object
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
                {
                    IInspectable inspectable = hit.transform.GetComponent<IInspectable>();
                    if (inspectable != null)
                    {
                        inspectable.OnInspect();
                        
                        inspectWindow = AttachUIWidget(inspectWindowPrefab);
                        inspectWindow.SetInspectable(inspectable);
                    }
                }
                
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
        camOffset.Offset.z += value * (scrollSpeed * 100f) * Time.deltaTime;
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
        else
        {
            SwitchState(PlayerState.Normal);
            
            AudioManager.PlayAudio(AudioManager.AudioType.TOWERS, towerBuildSFX, 1f, hitPosition);
        }
    }
}