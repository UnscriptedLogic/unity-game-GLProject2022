using Core.Pooling;
using EntityBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Units;

namespace Projectiles
{
    public class ProjectileSettings
    {
        private float damage;
        private float speed;
        private float lifetime;

        public float Damage => damage;
        public float Speed => speed;
        public float LifeTime => lifetime;

        public ProjectileSettings(float damage, float speed, float lifetime)
        {
            this.damage = damage;
            this.speed = speed;
            this.lifetime = lifetime;
        }
    }

    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        
        private float damage = 1f;
        private float movementSpeed = 10f;
        private float lifetime = 1f;

        private PoolManager poolManager;
        private MovementBehaviour movementBehaviour;
        private ProjectileSettings projectileSettings;

        public PoolManager PoolManager { get => poolManager; set { poolManager = value; } }
        public ProjectileSettings ProjectileSettings { get => projectileSettings; }

        private void Start() => movementBehaviour = new MovementBehaviour();

        private void OnEnable()
        {
            movementSpeed = 0f;
            lifetime = 10f;
        }

        public void Initialize(ProjectileSettings projectileSettings)
        {
            damage = projectileSettings.Damage;
            movementSpeed = projectileSettings.Speed;
            lifetime = projectileSettings.LifeTime;
        }

        private void Update()
        {
            movementBehaviour.MoveEntity(movementSpeed, transform.forward, rb);

            if (lifetime <= 0f)
            {
                poolManager.PushToPool(gameObject);
                projectileSettings = null;
            } else
            {
                lifetime -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && other.GetComponent<UnitMovement>() != null)
            {
                damageable.ModifyHealth(ModificationType.Subtract, damage);
                poolManager.PushToPool(gameObject);
                projectileSettings = null;
            }
        }
    }
}