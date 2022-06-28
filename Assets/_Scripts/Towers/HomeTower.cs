using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using Interfaces;
using Assets.Scripts.CustomEventSystem;
using System;

namespace Towers
{
    public class HomeTower : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health;
        [SerializeField] private float currHealth;
        [SerializeField] private VoidEventSO OnHealthDepleted;

        public Action<float> OnHealthModified;

        public float MaxHealth => health;
        public float CurrentHealth => currHealth;

        private void OnTriggerEnter(Collider other)
        {
            EnemyUnits unit = other.GetComponent<EnemyUnits>();
            if (unit != null)
            {
                ModifyHealth(ModificationType.Subtract, unit.CurrentHealth);
                unit.DestroyUnit();
            }
        }

        public void ModifyHealth(ModificationType modificationType, float amount, float maximum = 0, float minimum = 0)
        {
            switch (modificationType)
            {
                case ModificationType.Add:
                    currHealth += amount;
                    if (currHealth > maximum)
                        currHealth = maximum;
                    break;
                case ModificationType.Subtract:
                    currHealth -= amount;
                    if (currHealth <= 0f)
                    {
                        currHealth = 0f;
                        OnHealthDepleted.RaiseEvent();
                    }
                    break;
                case ModificationType.Set:
                    currHealth = amount;
                    break;
                case ModificationType.Divide:
                    currHealth = (currHealth / amount) > maximum ? maximum : currHealth;
                    currHealth = (currHealth / amount) < minimum ? minimum : currHealth;
                    break;
                case ModificationType.Multiply:
                    currHealth = (currHealth * amount) > maximum ? maximum : currHealth;
                    currHealth = (currHealth * amount) < minimum ? minimum : currHealth;
                    break;
                default:
                    break;
            }

            OnHealthModified?.Invoke(CurrentHealth);
        }
    }
}
