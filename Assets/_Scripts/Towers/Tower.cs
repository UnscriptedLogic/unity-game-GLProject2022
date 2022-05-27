using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using EntityBehaviours;
using Units;
using Projectiles;

namespace Towers
{
    public class Tower : MonoBehaviour, IFocuseable, IAttackable
    {
        [Header("Attributes")]
        [SerializeField] private float damage = 1f;
        [SerializeField] private float projectileSpeed = 30f;
        [SerializeField] private float projectileLifetime = 0.5f;

        [SerializeField] private float range = 5f;
        [SerializeField] private float attackInterval = 1f;
        [SerializeField] private float rotationSpeed = 5f;

        private float _attackInterval;
        private Collider[] unitsInRange;
        
        private UnitMovement target;
        private AttackBehaviour attackBehaviour;

        [Header("Others")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform shootAnchor;
        [SerializeField] private Transform rotatonPart;
        [SerializeField] private LayerMask unitLayer;

        private LookBehaviour lookBehaviour;

        [Header("Debugging")]
        [SerializeField] private bool drawGizmos;

        private void Start()
        {
            lookBehaviour = new LookBehaviour();
            attackBehaviour = new AttackBehaviour(Core.Pooling.PoolManager.instance);
        }

        private void Update()
        {
            unitsInRange = Physics.OverlapSphere(transform.position, range, unitLayer);
            foreach (Collider collider1 in unitsInRange)
            {
                UnitMovement unitMovement = collider1.GetComponent<UnitMovement>();
                if (target == null)
                {
                    target = unitsInRange[0].GetComponent<UnitMovement>();
                }
                else
                {
                    if (Vector3.Distance(target.transform.position, transform.position) > range)
                    {
                        target = null;
                        return;
                    }

                    if (unitMovement.WaypointIndex > target.WaypointIndex)
                        target = unitMovement;
                }
            }

            if (unitsInRange.Length > 0 && target != null)
            {
                FocusObject(rotatonPart, target.transform.position, rotationSpeed);
                if (_attackInterval <= 0f)
                {
                    DoAttack(projectilePrefab, shootAnchor);
                    _attackInterval = attackInterval;
                }
            }

            _attackInterval = _attackInterval > 0 ? _attackInterval -= Time.deltaTime : attackInterval;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.DrawWireSphere(transform.position, range);
        }

        public void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed) => lookBehaviour.FocusTarget(partToRotate, target, rotationSpeed);
        public void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            ProjectileSettings settings = new ProjectileSettings(damage, projectileSpeed, projectileLifetime);
            attackBehaviour.Attack(projectile, spawnpoint, settings);
        }
    }
}