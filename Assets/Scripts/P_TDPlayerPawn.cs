using UnityEngine;
using UnscriptedEngine;

public class P_TDPlayerPawn : URTSCamera
{
    [Header("Extension")]
    [SerializeField] private Vector2 panningDetectionThickness = new Vector2(5, 5);

    [Header("Scrolling")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private Vector2 scrollBounds;

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
}