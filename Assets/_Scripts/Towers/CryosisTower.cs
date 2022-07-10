using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class CryosisTower : Tower
    {
        [Header("Cryosis Extension")]
        [SerializeField] private float slowDuration = 10f;
        [SerializeField] private float slowPercent = 20f;
        [SerializeField] private float freezeChance = 1f;

        [SerializeField] private float critSlowAmount = 99f;
        [SerializeField] private float critDuration = 99f;

        public override void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            float slow = slowPercent;
            if (MathHelper.FromIntZeroTo(100) <= freezeChance)
            {
                slow = critSlowAmount;
                slowDuration = critDuration;
            }

            ProjectileSettings settings = new ProjectileSettings(damage, projectileSpeed, projectileLifetime, piercingPercent);
            SphericalStatusProjectile projectile1 = (SphericalStatusProjectile)attackBehaviour.Attack(projectile, spawnpoint, settings);
            projectile1.EffectAmount = slow;
            projectile1.EffectDuration = slowDuration;
            projectile1.TickSpeed = 1;
        }
    }
}