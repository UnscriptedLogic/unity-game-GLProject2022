using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 10f;
        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private int borderThickness = 5;
        [SerializeField] private Vector2 boundaries = new Vector2(50f, 50f);
        [SerializeField] private Vector2 scrollClamp = new Vector2(5, 50);
        [SerializeField] private Transform camTransform;

        private Vector3 pos;
        private float scroll;

        private void Update()
        {
            pos = transform.position;

            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= borderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - borderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= borderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }

            scroll = Input.GetAxis("Mouse ScrollWheel");
            camTransform.Translate(transform.forward * scroll * scrollSpeed * 100f * Time.deltaTime);

            pos.x = Mathf.Clamp(pos.x, -boundaries.x, boundaries.x);
            pos.z = Mathf.Clamp(pos.z, -boundaries.y, boundaries.y);

            transform.position = pos;
        }
    }
}