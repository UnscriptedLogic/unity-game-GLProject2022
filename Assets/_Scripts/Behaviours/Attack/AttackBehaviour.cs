using System;
using System.Collections;
using System.Collections.Generic;
using Core.Pooling;
using Projectiles;
using UnityEngine;

namespace EntityBehaviours
{
    public class AttackBehaviour
    {
        private PoolManager poolManager;

        public AttackBehaviour(PoolManager poolManager)
        {
            this.poolManager = poolManager;
        }

        public Projectile Attack(GameObject projectilePrefab, Transform spawnPoint, ProjectileSettings settings)
        {
            GameObject projectile = poolManager.PullFromPool(projectilePrefab);
            projectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            Projectile script = projectile.GetComponent<Projectile>();
            script.PoolManager = poolManager;
            script.Initialize(settings);

            return script;
        }
    }
}