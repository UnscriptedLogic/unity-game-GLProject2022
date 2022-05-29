using System;
using Core.Grid;
using Core.Pathing;
using Core.Pooling;
using Interfaces;
using UnityEngine;
using External.DamageFlash;
using External.CustomSlider;

namespace Units
{
    public class UnitMovement : MonoBehaviour, IDamageable
    {
        private PathManager nodeManager;
        private GridNode[] nodePath;

        [SerializeField] private DamageFlash damageFlash;

        [Header("Stats")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float movementSpeed = 3f;

        [SerializeField] private WorldSpaceCustomSlider healthbar;
        
        private float currHealth;
        private int waypointCounter = 0;

        public int WaypointIndex => waypointCounter;
        public Action<ModificationType, float> OnHealthModified;

        private void OnEnable()
        {
            InitializeEnemy();
        }

        public void InitializeEnemy()
        {
            nodeManager = PathManager.instance;
            nodePath = nodeManager.Path;
            transform.position = nodePath[0].Position;

            waypointCounter = 0;
            currHealth = health;

            healthbar.SetLimits(currHealth, health);
            healthbar.SetValue(health);
        }

        private void Update()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            if (waypointCounter >= nodePath.Length)
                return;

            Vector3 direction = GetWaypoint(waypointCounter) - transform.position;
            transform.Translate(direction.normalized * movementSpeed * Time.deltaTime, Space.World);
            transform.forward = direction;

            if (Vector3.Distance(transform.position, GetWaypoint(waypointCounter)) <= 0.05f)
                waypointCounter++;
        }

        private Vector3 GetWaypoint(int index)
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
                    currHealth -= amount;
                    healthbar.SetValue(currHealth);
                    if (currHealth <= 0f)
                        currHealth = 0f;
                    else
                        damageFlash.Flash();
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
                PoolManager.instance.PushToPool(gameObject);
                transform.position = nodePath[0].Position;
            }
        }
    }
}