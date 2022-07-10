using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Units;
using Game.StatusEffects;

namespace Projectiles
{
    public class SphericalStatusProjectile : Projectile
    {
        [SerializeField] private AffectorManager.EffectType effectType;
        [SerializeField] private float sizeMultiplier = 3f;
        [SerializeField] private AnimationCurve sizeCurve;

        private float duration;
        private float effectAmount;
        private float tickSpeed;
        [SerializeField] private Vector3 startSize = Vector3.one;

        public float TickSpeed { get => tickSpeed; set { tickSpeed = value; } }
        public float EffectAmount { get => effectAmount; set { effectAmount = value; } }
        public float EffectDuration { get => duration; set { duration = value; } } 

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
                damageable.ModifyHealth(ModificationType.Subtract, base.damage);

                AffectorManager affectorManager = other.transform.GetComponent<AffectorManager>();
                if (affectorManager != null)
                affectorManager.ApplyEffect(effectType, duration, effectAmount, tickSpeed);
            }
        }

        protected override void OnValidate()
        {

        }
    }
}