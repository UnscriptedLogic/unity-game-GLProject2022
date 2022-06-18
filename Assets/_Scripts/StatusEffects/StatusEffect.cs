using Core.Assets;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class StatusEffect : MonoBehaviour
    {
        protected float duration;
        protected float amount;
        protected float tickInterval;

        protected float _duration;
        protected float _tick;
        protected bool isInifinite;
        protected AffectorManager affectorManager;
        protected GameObject effect;
        protected IDamageable damageable;

        public float TickInterval => tickInterval;
        public float Amount => amount;
        public float Duration => duration;

        public virtual void Initialize(AffectorManager affectorManager, float duration, float amount, float tickInterval)
        {
            this.affectorManager = affectorManager;
            this.duration = duration;
            this.amount = amount;
            this.tickInterval = tickInterval;
            damageable = GetComponent<IDamageable>();

            _duration = duration;
            if (duration == 0)
            {
                isInifinite = true;
            }

            if (AssetManager.instance.FlameParticle != null)
            {
                effect = Instantiate(AssetManager.instance.FlameParticle, transform);
            }
        }

        protected virtual void Update()
        {
            ReduceDuration();

            if (_tick <= 0f)
            {
                Tick();
                _tick = tickInterval;
            }
            else
            {
                _tick -= Time.deltaTime;
            }
        }

        protected virtual void Tick()
        {

        }

        public void ResetDuration(float newDuration)
        {
            _duration = newDuration;
        }

        private void ReduceDuration()
        {
            if (isInifinite)
                return;

            if (_duration <= 0f)
            {
                DestroyStatus();
            }
            else
            {
                _duration -= Time.deltaTime;
            }
        }

        private void OnDisable()
        {
            DestroyStatus();
        }

        private void OnDestroy()
        {
            DestroyStatus();
        }

        private void DestroyStatus()
        {
            affectorManager.Remove(this);
            if (effect != null)
                Destroy(effect.gameObject);
            Destroy(this);
        }
    }
}