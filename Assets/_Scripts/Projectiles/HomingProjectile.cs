using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Projectiles
{
    public class HomingProjectile : Projectile
    {
        [Header("Homing Extension")]
        [SerializeField] private LayerMask enemyLayer;

        private Transform target;

        public override void Initialize(ProjectileSettings projectileSettings)
        {
            base.Initialize(projectileSettings);

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.SphereCast(ray, 0.25f, out RaycastHit hitInfo, 1000f, enemyLayer))
            {
                if (hitInfo.transform.GetComponent<EnemyUnits>() != null)
                {
                    target = hitInfo.transform;
                }
            }
        }

        protected override void Update()
        {
            if (target != null && target.gameObject.activeInHierarchy == true)
            {
                Vector3 dir = new Vector3(target.position.x, target.position.y + 0.5f, target.position.z) - transform.position;
                float distPerFrame = movementSpeed * Time.deltaTime;

                transform.Translate(dir.normalized * distPerFrame, Space.World);

                if (lifetime <= 0f)
                {
                    DestroyProjectile();
                }
                else
                {
                    lifetime -= Time.deltaTime;
                }
            }
            else
            {
                movementBehaviour.MoveEntity(movementSpeed, transform.forward, rb);
            }

        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.DrawSphere(target.position, 0.5f);
            }
        }
    }
}