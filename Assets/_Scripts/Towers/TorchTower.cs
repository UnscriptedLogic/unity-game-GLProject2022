using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class TorchTower : Tower
    {
        [Header("Torch")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float burnDamage = 5;
        [SerializeField] private float burnDuration = 5;
        [SerializeField] private float tickSpeed = 1;

        public override void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed)
        {

        }

        public override void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                ProjectileSettings settings = new ProjectileSettings(damage, projectileSpeed, projectileLifetime);
                FlameProjectile projectile1 = (FlameProjectile)attackBehaviour.Attack(projectile, spawnPoints[i], settings);
                projectile1.BurnDamage = burnDamage;
                projectile1.BurnDuration = burnDuration;
                projectile1.TickSpeed = tickSpeed;
            }
        }
    }
}