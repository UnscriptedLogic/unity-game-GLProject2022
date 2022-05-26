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

        public void Attack(GameObject projectilePrefab, Transform spawnPoint)
        {
            GameObject projectile = poolManager.PullFromPool(projectilePrefab);
            projectile.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            projectile.GetComponent<Projectile>().PoolManager = poolManager;
        }
    }
}