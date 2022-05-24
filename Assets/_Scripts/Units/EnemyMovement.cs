using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Pooling;
using Core.Pathing;
using Core.Grid;

namespace Units
{
    public class EnemyMovement : MonoBehaviour, ITakeDamage
    {
        private PathManager nodeManager;
        private GridNode[] nodePath;

        [Header("Stats")]
        public float health;
        [HideInInspector] public float currHealth;
        public float movementSpeed = 3f;

        public int waypointCounter;

        private void OnEnable()
        {
            InitializeEnemy();
        }

        public void InitializeEnemy()
        {
            nodeManager = PathManager.instance;
            nodePath = nodeManager.Path;
            transform.position = nodePath[0].NodeObject.transform.position;

            waypointCounter = 0;
            currHealth = health;
        }

        private void Update()
        {
            if (waypointCounter >= nodePath.Length)
            {
                return;
            }

            Vector3 direction = GetWaypoint(waypointCounter) - transform.position;
            transform.Translate(direction.normalized * movementSpeed * Time.deltaTime, Space.World);
            transform.forward = direction;

            if (Vector3.Distance(transform.position, GetWaypoint(waypointCounter)) <= 0.05f)
            {
                waypointCounter++;
            }
        }

        private Vector3 GetWaypoint(int index)
        {
            Vector3 position = nodePath[index].NodeObject.transform.position;
            position.y = transform.position.y;
            return position;
        }

        public void TakeDamage(float amount)
        {
            currHealth -= amount;
            if (currHealth <= 0f)
            {
                PoolManager.instance.PushToPool(gameObject);
                //transform.position = nodePath[0].transform.position;
            }
        }
    }

}