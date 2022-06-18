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

        [Serializable]
        public class Resistance
        {
            [SerializeField] private EffectType effectType;
            [Range(-200f, 200f)]
            [SerializeField] private float resistance;

            public EffectType EffectType => effectType;
            public float EffectResistance => resistance;
        }

        private List<StatusEffect> statusEffects = new List<StatusEffect>();
        public List<StatusEffect> StatusEffects => statusEffects;

        [SerializeField] private Resistance[] resistances;

        private void OnEnable()
        {
            PurgeEffects();
        }

        public void ApplyEffect(EffectType type, float duration, float amount, float tick)
        {
            if (hasResistance(type, out float resistance))
            {
                //Resistant to effect
                if (resistance > 0)
                {
                    duration *= resistance;
                    amount *= resistance;
                }
                //Weak to effect
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
                    if (StatusEffectExists(out StatusBurning status))
                    {
                        //This checks if there is a status weaker than the current and overrides it
                        if (status.Amount < amount)
                        {
                            status.Initialize(this, duration, amount, tick);
                        } else
                        {
                            status.ResetDuration(duration);
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
                Destroy(statusEffects[i]);
            }
        }

        public void Remove(StatusEffect statusEffect)
        {
            Destroy(statusEffect);
            statusEffects.Remove(statusEffect);
        }
    }
}