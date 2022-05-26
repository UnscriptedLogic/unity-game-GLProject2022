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
        [SerializeField] private float lifeTime = 1f;
        [SerializeField] private Rigidbody rb;

        private float _lifetime;
        private PoolManager poolManager;
        private MovementBehaviour movementBehaviour;

        public PoolManager PoolManager { get => poolManager; set { poolManager = value; } }

        private void OnEnable()
        {
            _lifetime = lifeTime;
            movementBehaviour = new MovementBehaviour();
        }

        private void Update()
        {
            movementBehaviour.MoveEntity(movementSpeed, transform.forward, rb);

            if (_lifetime <= 0)
                poolManager.PushToPool(gameObject);
            else
                _lifetime -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            poolManager.PushToPool(gameObject);
        }
    }
}