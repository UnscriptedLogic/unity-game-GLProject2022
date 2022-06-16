using Game.StatusEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class AffectorManager : MonoBehaviour
    {
        public enum EffectType
        {
            Burn
        }

        [SerializeField] private List<StatusEffect> statusEffects;
        public List<StatusEffect> StatusEffects => statusEffects;

        private void OnEnable()
        {
            PurgeEffects();
        }

        public void ApplyEffect(EffectType type, float duration, float amount, float tick)
        {
            StatusEffect statusEffect = null;

            switch (type)
            {
                case EffectType.Burn:
                    if (StatusEffectExists(out StatusBurning status))
                    {
                        //This checks if there is a status weaker than the current and overrides it
                        if (status.Amount <= amount)
                        {
                            status.Initialize(this, duration, amount, tick);
                        }
                    }
                    else
                    {
                        statusEffect = gameObject.AddComponent<StatusBurning>();
                        statusEffect.Initialize(this, duration, amount, tick);
                        statusEffects.Add(statusEffect);
                    }

                    break;
                default:
                    break;
            }
        }

        private bool StatusEffectExists<T>(out T statusEffect)
        {
            statusEffect = gameObject.GetComponent<T>();
            return statusEffect != null;
        }

        public void PurgeEffects()
        {
            for (int i = 0; i < statusEffects.Count; i++)
            {
                Destroy(statusEffects[i]);
            }
        }

        public void Remove(StatusEffect statusEffect)
        {
            statusEffects.Remove(statusEffect);
        }
    }
}