using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Units;
using Game.StatusEffects;

namespace Projectiles
{
    public class FlameProjectile : Projectile
    {
        [SerializeField] private float sizeMultiplier = 3f;
        [SerializeField] private AnimationCurve sizeCurve;

        [SerializeField] private float burnDuration;
        [SerializeField] private float burnDamage;
        [SerializeField] private float tickSpeed;
        [SerializeField] private Vector3 startSize = Vector3.one;

        public float TickSpeed { get => tickSpeed; set { tickSpeed = value; } }
        public float BurnDamage { get => burnDamage; set { burnDamage = value; } }
        public float BurnDuration { get => burnDuration; set { burnDuration = value; } } 

        public override void Initialize(ProjectileSettings projectileSettings)
        {
            base.Initialize(projectileSettings);
            transform.localScale = startSize;
            sizeCurve.keys[sizeCurve.keys.Length - 1].time = projectileSettings.LifeTime;
        }

        protected override void Update()
        {
            base.Update();


            if (projectileSettings == null)
                return;

            movementSpeed = (projectileSettings.Speed / 100f) * (lifetime / projectileSettings.LifeTime * 100f);
            float scale = sizeCurve.Evaluate(projectileSettings.LifeTime - lifetime) * sizeMultiplier;
            transform.localScale = Vector3.one * scale;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && other.GetComponent<Unit>() != null)
            {
                damageable.ModifyHealth(ModificationType.Subtract, damage);

                AffectorManager affectorManager = other.transform.GetComponent<AffectorManager>();
                if (affectorManager != null)
                affectorManager.ApplyEffect(AffectorManager.EffectType.Burn, burnDuration, burnDamage, tickSpeed);
            }
        }

        protected override void OnValidate()
        {

        }
    }
}