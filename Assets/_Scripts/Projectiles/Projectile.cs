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
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected TrailRenderer trailRenderer;
        
        protected float damage = 1f;
        protected float movementSpeed = 10f;
        protected float lifetime = 1f;

        protected PoolManager poolManager;
        protected MovementBehaviour movementBehaviour;
        protected ProjectileSettings projectileSettings;

        public PoolManager PoolManager { get => poolManager; set { poolManager = value; } }
        public ProjectileSettings ProjectileSettings { get => projectileSettings; }

        protected void Start() => movementBehaviour = new MovementBehaviour();

        protected void OnEnable()
        {
            movementSpeed = 0f;
            lifetime = 10f;
        }

        public virtual void Initialize(ProjectileSettings projectileSettings)
        {
            this.projectileSettings = projectileSettings;
            damage = projectileSettings.Damage;
            movementSpeed = projectileSettings.Speed;
            lifetime = projectileSettings.LifeTime;
        }

        protected virtual void Update()
        {
            movementBehaviour.MoveEntity(movementSpeed, transform.forward, rb);

            if (lifetime <= 0f)
            {
                DestroyProjectile();
            } else
            {
                lifetime -= Time.deltaTime;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && other.GetComponent<Unit>() != null)
            {
                damageable.ModifyHealth(ModificationType.Subtract, damage);
                DestroyProjectile();
            }
        }

        protected virtual void DestroyProjectile()
        {
            poolManager.PushToPool(gameObject);
            trailRenderer.Clear();
            projectileSettings = null;
        }

        protected virtual void OnValidate()
        {
            if (trailRenderer == null)
                trailRenderer.TryGetComponent(out trailRenderer);
        }
    }
}