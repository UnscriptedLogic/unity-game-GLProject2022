using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Standalone
{
    public class StartCameraMovement : MonoBehaviour
    {
        [SerializeField] private float speed;

        private void Update()
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }
}