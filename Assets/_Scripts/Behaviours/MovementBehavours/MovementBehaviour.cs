using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviours
{
    public class MovementBehaviour
    {
        public void MoveEntity(float speed, Vector3 direction, Rigidbody rb)
        {
            rb.MovePosition(rb.transform.position + (direction * speed * Time.fixedDeltaTime));
        }
    }
}