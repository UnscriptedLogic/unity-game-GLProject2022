using Core.Pooling;
using EntityBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private Rigidbody rb;
        private PoolManager poolManager;

        public PoolManager PoolManager { get => poolManager; set { poolManager = value; } }

        private MovementBehaviour movementBehaviour;

        private void Start()
        {
            movementBehaviour = new MovementBehaviour();
        }

        private void Update()
        {
            movementBehaviour.MoveEntity(movementSpeed, transform.forward, rb);
        }

        private void OnTriggerEnter(Collider other)
        {
            poolManager.PushToPool(gameObject);
        }
    }
}