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
        [SerializeField] protected int towerID = 0;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float projectileSpeed = 30f;
        [SerializeField] protected float projectileLifetime = 0.5f;
        [SerializeField] protected float piercingPercent = 0f;

        [SerializeField] protected float range = 5f;
        [SerializeField] protected float attackInterval = 1f;
        [SerializeField] protected float rotationSpeed = 5f;
        [SerializeField] protected bool multiAxis = false;

        protected float _attackInterval;
        protected Collider[] unitsInRange;

        protected Unit target;
        protected AttackBehaviour attackBehaviour;

        [Header("Others")]
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected Transform shootAnchor;
        [SerializeField] protected Transform rotationPart;
        [SerializeField] protected List<GridNode> ownedGridNode;
        [SerializeField] protected LayerMask unitLayer;
        [SerializeField] protected LayerMask wallLayer;

        protected LookBehaviour lookBehaviour;

        [Header("Debugging")]
        [SerializeField] protected bool drawGizmos;

        public int ID => towerID;
        public float Damage => damage;
        public float Range => range;
        public float FireRate => attackInterval;
        public float TurnSpeed => rotationSpeed;
        public float Piercing => piercingPercent;
        public float ProjSpeed => projectileSpeed;
        public float ProjLifeTime => projectileLifetime;
        public List<GridNode> OwnedGridNodes { get => ownedGridNode; set { ownedGridNode = value; } }

        protected virtual void Start()
        {
            lookBehaviour = new LookBehaviour();
            attackBehaviour = new AttackBehaviour(PoolManager.instance);
        }

        protected virtual void Update()
        {
            if (_attackInterval >= 0)
            {
                _attackInterval -= Time.deltaTime;
            }

            if (_attackInterval <= 0f)
            {
                Vector3 adjustedCenter = new Vector3(transform.position.x, 0.2f, transform.position.z);
                unitsInRange = Physics.OverlapSphere(adjustedCenter, range, unitLayer);
                foreach (Collider collider1 in unitsInRange)
                {
                    EnemyUnits unitMovement = collider1.GetComponent<EnemyUnits>();
                    if (unitMovement != null)
                    {
                        if (target == null)
                        {
                            target = unitsInRange[0].GetComponent<EnemyUnits>();
                        }
                        else
                        {
                            if (Vector3.Distance(target.transform.position, adjustedCenter) > range || target.WaypointIndex == 0)
                            {
                                if (unitsInRange.Length > 1)
                                {
                                    target = unitsInRange[1].GetComponent<EnemyUnits>();
                                }
                                else
                                {
                                    target = null;
                                }
                                return;
                            }

                            //Switch targets if one is ahead of the other
                            if (unitMovement.WaypointIndex > target.WaypointIndex)
                                target = unitMovement;
                        }   
                    }
                }

                if (unitsInRange.Length > 0 && target != null)
                {
                    //Vector3 correctedOrigin = new Vector3(transform.position.x, shootAnchor.position.y, transform.position.z);
                    //Ray ray = new Ray(correctedOrigin, target.transform.position - correctedOrigin);
                    //if (!Physics.Raycast(ray, range, wallLayer))
                    //{

                    //}

                    FocusObject(rotationPart, target.transform.position, rotationSpeed);
                    DoAttack(projectilePrefab, shootAnchor);
                    _attackInterval = attackInterval;
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.DrawWireSphere(transform.position, range);
        }

        public virtual void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed)
        {
            if (!multiAxis)
            {
                rotationPart.LookAt(new Vector3(target.x, partToRotate.position.y, target.z));
            }
            else
            {
                rotationPart.LookAt(new Vector3(target.x, target.y + 0.5f, target.z));
            }
        }

        public virtual void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            ProjectileSettings settings = new ProjectileSettings(damage, projectileSpeed, projectileLifetime, piercingPercent);
            attackBehaviour.Attack(projectile, spawnpoint, settings);
        }

        public void RemoveSelf()
        {
            ownedGridNode[0].RemoveTower();
            for (int i = 1; i < ownedGridNode.Count; i++)
            {
                ownedGridNode[i].isObstacle = false;
            }
        }
    }
}