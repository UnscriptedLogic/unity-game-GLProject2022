using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using EntityBehaviours;
using Units;
using Projectiles;
using Core.Pooling;
using Core.Grid;
using System.Linq;

namespace Towers
{
    public class Tower : MonoBehaviour, IFocuseable, IAttackable
    {
        public enum TargettingMode
        {
            First,
            Last,
            Closest,
            Furthest,
            Weakest,
            Strongest
        }

        [Header("Attributes")]
        [SerializeField] protected int towerID = 0;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float projectileSpeed = 30f;
        [SerializeField] protected float projectileLifetime = 0.5f;
        [SerializeField] protected float piercingPercent = 0f;
        [SerializeField] protected TargettingMode targettingMode = TargettingMode.First;
        [SerializeField] protected float range = 5f;
        [SerializeField] protected float attackInterval = 1f;
        [SerializeField] protected float rotationSpeed = 5f;
        [SerializeField] protected bool multiAxis = false;

        protected float _attackInterval;
        protected Collider[] unitsInRange;
        protected List<EnemyUnits> targetsInRange = new List<EnemyUnits>();

        protected Unit currentTarget;
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

        protected virtual void FixedUpdate()
        {
            if (currentTarget != null)
            {
                if (LostTarget())
                {
                    currentTarget = null;
                    return;
                }
            }

            if (targetsInRange.Count > 0)
            {
                targetsInRange.Clear();
            }

            Vector3 adjustedCenter = new Vector3(transform.position.x, 0.2f, transform.position.z);
            unitsInRange = Physics.OverlapSphere(adjustedCenter, range, unitLayer);
            foreach (Collider collider1 in unitsInRange)
            {
                EnemyUnits unitMovement = collider1.GetComponent<EnemyUnits>();
                if (unitMovement == null) continue;

                if (Vector3.Distance(unitMovement.transform.position, adjustedCenter) > range || unitMovement.WaypointIndex == 0) continue;

                targetsInRange.Add(unitMovement);
            }

            if (targetsInRange.Count > 0)
            {
                SortTargets();
            }
        }

        protected void SortTargets()
        {
            EnemyUnits target = null;

            switch (targettingMode)
            {
                case TargettingMode.First:
                    //Node closest to the end point
                    target = targetsInRange.OrderByDescending(t => t.GetComponent<EnemyUnits>().WaypointIndex).FirstOrDefault();
                    break;
                case TargettingMode.Last:
                    //Node furthest from the end point
                    target = targetsInRange.OrderBy(t => t.GetComponent<EnemyUnits>().WaypointIndex).FirstOrDefault();
                    break;
                case TargettingMode.Closest:
                    //Closest distance to tower
                    target = targetsInRange.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();
                    break;
                case TargettingMode.Furthest:
                    //Furthest distance to tower
                    target = targetsInRange.OrderByDescending(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();
                    break;
                case TargettingMode.Strongest:
                    //Highest health point in range
                    target = targetsInRange.OrderByDescending(t => t.GetComponent<EnemyUnits>().CurrentHealth).FirstOrDefault();
                    break;
                case TargettingMode.Weakest:
                    //Lowest health point in range
                    target = targetsInRange.OrderBy(t => t.GetComponent<EnemyUnits>().CurrentHealth).FirstOrDefault();
                    break;
                default:
                    break;
            }

            if (currentTarget == null || target != currentTarget)
            {
                currentTarget = target;
            }
        }

        protected virtual void Update()
        {
            if (_attackInterval >= 0f)
            {
                _attackInterval -= Time.deltaTime;
            }

            if (_attackInterval > 0f) return;

            if (currentTarget == null) return;

            FocusObject(rotationPart, currentTarget.transform.position, rotationSpeed);
            DoAttack(projectilePrefab, shootAnchor);
            _attackInterval = attackInterval;
        }

        protected bool LostTarget()
        {
            if (!currentTarget.gameObject.activeInHierarchy)
            {
                return true;
            }

            if (Vector3.Distance(currentTarget.transform.position, transform.position) >= range + 1f)
            {
                return true;
            }

            return false;
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