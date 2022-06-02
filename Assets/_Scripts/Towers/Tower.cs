using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using EntityBehaviours;
using Units;
using Projectiles;
using Core.Pooling;
using Core.Grid;

namespace Towers
{
    public class Tower : MonoBehaviour, IFocuseable, IAttackable
    {
        [Header("Attributes")]
        [SerializeField] private int towerID = 0;
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
        [SerializeField] private Transform rotationPart;
        [SerializeField] private GridNode gridNode;
        [SerializeField] private LayerMask unitLayer;

        private LookBehaviour lookBehaviour;

        [Header("Debugging")]
        [SerializeField] private bool drawGizmos;

        public int ID => towerID;
        public float Damage => damage;
        public float Range => range;
        public float FireRate => attackInterval;
        public float TurnSpeed => rotationSpeed;
        public float ProjSpeed => projectileSpeed;
        public GridNode GridNode { get => gridNode; set { gridNode = value; } }

        private void Start()
        {
            lookBehaviour = new LookBehaviour();
            attackBehaviour = new AttackBehaviour(PoolManager.instance);
        }

        private void Update()
        {
            if (_attackInterval >= 0)
            {
                _attackInterval -= Time.deltaTime;
            }

            if (_attackInterval <= 0f)
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
                        if (Vector3.Distance(target.transform.position, transform.position) > range || target.WaypointIndex == 0)
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
                    rotationPart.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    DoAttack(projectilePrefab, shootAnchor);
                    _attackInterval = attackInterval;
                }
            }
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

        public void RemoveSelf() => gridNode.RemoveTower();
    }
}