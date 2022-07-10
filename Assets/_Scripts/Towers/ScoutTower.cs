using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class ScoutTower : Tower
    {
        [Header("Scout")]
        [SerializeField] private Transform[] spawnPoints;

        public override void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            ProjectileSettings settings = new ProjectileSettings(damage, projectileSpeed, projectileLifetime, piercingPercent);
            attackBehaviour.Attack(projectile, spawnPoints[0], settings);

            ProjectileSettings spreadSettings = new ProjectileSettings(damage / 2f, projectileSpeed, projectileLifetime, piercingPercent);
            for (int i = 1; i < spawnPoints.Length; i++)
            {
                attackBehaviour.Attack(projectile, spawnPoints[i], spreadSettings);
            }
        }
    }
}