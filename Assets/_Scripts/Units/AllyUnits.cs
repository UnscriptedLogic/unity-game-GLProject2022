using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class AllyUnits : Unit
    {
        protected override void Update()
        {
            if (WaypointIndex == Path.Length - 1)
            {
                DestroyUnit();
            }

            base.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyUnits enemyUnit))
            {
                float enemyHealth = enemyUnit.CurrentHealth;
                float damage = currHealth;
                if (enemyHealth > currHealth)
                {
                    damage = currHealth;
                }
                else if (enemyHealth < currHealth)
                {
                    damage = enemyHealth;
                }

                enemyUnit.ModifyHealth(ModificationType.Subtract, damage);
                ModifyHealth(ModificationType.Subtract, damage);
            }
        }
    }
}