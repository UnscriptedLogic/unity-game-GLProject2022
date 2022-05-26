using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using EntityBehaviours;
using Units;

namespace Towers
{
    public class Tower : MonoBehaviour, IFocuseable, IAttackable
    {
        [Header("Attributes")]
        [SerializeField] private float range = 5f;
        [SerializeField] private float attackInterval = 1f;
        [SerializeField] private float rotationSpeed = 5f;

        private float _attackInterval;
        private List<UnitMovement> unitsInRange;
        
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, unitLayer);
            foreach (Collider collider in colliders)
            {
                UnitMovement unitMovement = collider.GetComponent<UnitMovement>();
                if (target == null)
                {
                    target = unitMovement;
                }
                else
                {
                    if (unitMovement.WaypointIndex > target.WaypointIndex)
                        if (unitMovement != target)
                            target = unitMovement;
                }
            }

            if (colliders.Length > 0)
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

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;

            Gizmos.DrawWireSphere(transform.position, range);
        }

        public void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed) => lookBehaviour.FocusTarget(partToRotate, target, rotationSpeed);
        public void DoAttack(GameObject projectile, Transform spawnpoint) => attackBehaviour.Attack(projectile, spawnpoint);
    }
}