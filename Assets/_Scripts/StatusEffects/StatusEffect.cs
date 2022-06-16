using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class StatusEffect : MonoBehaviour
    {
        [SerializeField] protected float duration;
        [SerializeField] protected float amount;
        [SerializeField] protected float tickInterval;
        [SerializeField] private GameObject effectsPrefab;

        protected float _duration;
        protected float _tick;
        protected bool isInifinite;
        protected AffectorManager affectorManager;
        protected GameObject effect;
        protected IDamageable damageable;

        public float TickInterval => tickInterval;
        public float Amount => amount;
        public float Duration => duration;
        public GameObject EffectsPrefab => effectsPrefab;

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

            if (effect != null)
                effect = Instantiate(effectsPrefab, transform);
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

        private void OnDestroy()
        {
            DestroyStatus();
        }

        private void DestroyStatus()
        {
            affectorManager.Remove(this);
            Destroy(effect);
            Destroy(this);
        }
    }
}