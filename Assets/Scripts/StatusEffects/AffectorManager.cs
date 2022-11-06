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
            Burn,
            Slow
        }

        [Serializable]
        public class Resistance
        {
            [SerializeField] private EffectType effectType;
            [Range(-400f, 400f)]
            [SerializeField] private float resistance;

            public EffectType EffectType => effectType;
            public float EffectResistance => resistance;
        }

        private List<StatusEffect> statusEffects = new List<StatusEffect>();
        public List<StatusEffect> StatusEffects => statusEffects;

        [SerializeField] private Resistance[] resistances;

        private void Start() => PurgeEffects();
        private void OnEnable() => PurgeEffects();
        private void OnDisable() => PurgeEffects();

        public void ApplyEffect(EffectType type, float duration, float amount, float tick)
        {
            if (hasResistance(type, out float resistance))
            {
                //Weak to effect
                if (resistance > 0)
                {
                    duration *= resistance;
                    amount *= resistance;
                }
                //Resistant to effect
                else if (resistance < 0)
                {
                    duration /= resistance;
                    amount /= resistance;
                }
                //Immune to effect
                else
                {
                    return;
                }
            }

            StatusEffect statusEffect = null;
            switch (type)
            {
                case EffectType.Burn:
                    if (StatusEffectExists(out StatusSlow slow))
                    {
                        RemoveStatusEffect(slow);
                    }

                    if (StatusEffectExists(out StatusBurning burnStatus))
                    {
                        //This checks if there is a status weaker than the current and overrides it
                        if (burnStatus.Amount < amount)
                        {
                            burnStatus.Initialize(this, duration, amount, tick);
                        } else
                        {
                            burnStatus.ResetDuration(duration);
                        }
                    }
                    else
                    {
                        statusEffect = gameObject.AddComponent<StatusBurning>();
                        statusEffect.Initialize(this, duration, amount, tick);
                        statusEffects.Add((StatusBurning)statusEffect);
                    }

                    break;

                case EffectType.Slow:

                    if (StatusEffectExists(out burnStatus))
                    {
                        RemoveStatusEffect(burnStatus);
                    }

                    if (StatusEffectExists(out StatusSlow slowStatus))
                    {

                        if (slowStatus.Amount < amount)
                        {
                            slowStatus.Initialize(this, duration, amount, tick);
                        }
                        else
                        {
                            slowStatus.ResetDuration(duration);
                        }
                    }
                    else
                    {
                        statusEffect = gameObject.AddComponent<StatusSlow>();
                        statusEffect.Initialize(this, duration, amount, tick);
                        statusEffects.Add((StatusSlow)statusEffect);
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

        private bool hasResistance(EffectType type, out float resistance)
        {
            for (int i = 0; i < resistances.Length; i++)
            {
                if (resistances[i].EffectType == type)
                {
                    resistance = (100 - resistances[i].EffectResistance) / 100f;
                    return true;
                }
            }

            resistance = 0f;
            return false;
        }

        public void PurgeEffects()
        {
            for (int i = 0; i < statusEffects.Count; i++)
            {
                RemoveStatusEffect(statusEffects[i]);
            }
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            statusEffect.DestroyStatus();
            statusEffects.Remove(statusEffect);
        }
    }
}