using System;
using Core.Grid;
using Core.Pathing;
using Core.Pooling;
using Interfaces;
using UnityEngine;
using External.DamageFlash;
using External.CustomSlider;
using Game;

namespace Units
{
    public class Unit : MonoBehaviour, IDamageable
    {
        protected PathManager nodeManager;
        protected GridNode[] nodePath;

        [SerializeField] protected DamageFlash damageFlash;

        [Header("Stats")]
        [SerializeField] protected float health = 100f;
        [SerializeField] protected float movementSpeed = 3f;

        [Space(10)]
        [SerializeField] protected float waypointVerifyDistance = 0.05f;
        [SerializeField] protected WorldSpaceCustomSlider healthbar;

        protected float currHealth;
        protected int waypointCounter = 0;
        protected LevelManager levelManager;

        public int WaypointIndex => waypointCounter;
        public float CurrentHealth => currHealth;
        public float MaxHealth => health;
        public float Speed { get => movementSpeed; set { movementSpeed = value; } }
        public LevelManager LevelManager => levelManager;
        public Action<ModificationType, float> OnHealthModified;
        protected bool initialized;

        public virtual void InitializeEnemy(LevelManager levelManager, int position = 0)
        {
            this.levelManager = levelManager;

            nodeManager = levelManager.PathManager;
            nodePath = nodeManager.Path;
            transform.position = nodePath[position].Position;

            waypointCounter = position;
            currHealth = health;

            if (healthbar)
            {
                healthbar.SetLimits(currHealth, health);
                healthbar.SetValue(health);
            }

            gameObject.SetActive(true);
            initialized = true;
        }

        protected virtual void Update()
        {
            if (!initialized)
                return;

            ApplyMovement();
        }

        protected void ApplyMovement()
        {
            if (waypointCounter >= nodePath.Length)
                return;

            Vector3 direction = GetWaypoint(waypointCounter) - transform.position;
            transform.Translate(direction.normalized * movementSpeed * Time.deltaTime, Space.World);
            //transform.forward = direction;

            if (Vector3.Distance(transform.position, GetWaypoint(waypointCounter)) <= waypointVerifyDistance)
                waypointCounter++;
        }

        protected Vector3 GetWaypoint(int index)
        {
            Vector3 position = nodePath[index].NodeObject.transform.position;
            position.y = transform.position.y;
            return position;
        }

        public void ModifyHealth(ModificationType modificationType, float amount, float maximum = 0f, float minimum = 0f)
        {
            switch (modificationType)
            {
                case ModificationType.Add:
                    currHealth += amount;
                    if (currHealth > maximum)
                        currHealth = maximum;
                    break;
                case ModificationType.Subtract:
                    float prev = currHealth;
                    currHealth -= amount;
                    healthbar.SetValue(currHealth);
                    if (currHealth <= 0f)
                        currHealth = 0f;
                    else
                        damageFlash.Flash();

                    levelManager.CurrencyManager.ModifyCurrency(ModificationType.Add, Mathf.Round(prev - currHealth));
                    break;
                case ModificationType.Set:
                    currHealth = amount;
                    break;
                case ModificationType.Divide:
                    currHealth = (currHealth / amount) > maximum ? maximum : currHealth;
                    currHealth = (currHealth / amount) < minimum ? minimum : currHealth;
                    break;
                case ModificationType.Multiply:
                    currHealth = (currHealth * amount) > maximum ? maximum : currHealth;
                    currHealth = (currHealth * amount) < minimum ? minimum : currHealth;
                    break;
                default:
                    break;
            }

            OnHealthModified?.Invoke(modificationType, amount);

            if (currHealth <= 0f)
            {
                DestroyUnit();
            }
        }

        public void DestroyUnit()
        {
            PoolManager.instance.PushToPool(gameObject);
            transform.position = nodePath[0].Position;
            waypointCounter = 0;
        }
    }
}