using Core.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class StatusBurning : StatusEffect
    {
        protected override void OnEffectStarted()
        {
            if (AssetManager.instance.FlameParticle != null)
            {
                effect = Instantiate(AssetManager.instance.FlameParticle, transform);
            }
        }

        protected override void Tick()
        {
            damageable.ModifyHealth(ModificationType.Subtract, amount);
        }
    }
}