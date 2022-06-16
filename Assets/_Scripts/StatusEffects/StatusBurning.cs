using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class StatusBurning : StatusEffect
    {
        protected override void Tick()
        {
            damageable.ModifyHealth(ModificationType.Subtract, amount);
        }
    }
}