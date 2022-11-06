using Core.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.StatusEffects
{
    public class StatusSlow : StatusEffect
    {
        private float originalSpeed;

        protected override void OnEffectStarted()
        {
            if (AssetManager.instance.FlameParticle != null)
            {
                effect = Instantiate(AssetManager.instance.IceParticle, transform);
            }

            originalSpeed = unit.Speed;
            unit.Speed = unit.Speed / 100f * (100 - amount);
        }

        protected override void OnEffectEnded()
        {
            unit.Speed = originalSpeed;
        }
    }
}