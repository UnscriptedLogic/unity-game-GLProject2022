using Core.Grid;
using Core.Pooling;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Towers
{
    public class EngiFactoryTower : Tower, IRequirePath
    {
        [Header("Engineer's Factory Extension")]
        [SerializeField] private GameObject unitPrefab;

        private PoolManager poolManager;
        private GridNode[] path;

        protected override void Start()
        {
            base.Start();
            poolManager = PoolManager.instance;
        }

        protected override void Update()
        {
            if (_attackInterval <= 0f)
            {
                GameObject unit = poolManager.PullFromPool(unitPrefab);
                unit.GetComponent<Unit>().InitializeEnemy(path, 0);
                _attackInterval = attackInterval;
            }
            else
            {
                _attackInterval -= Time.deltaTime;
            }
        }

        public override void DoAttack(GameObject projectile, Transform spawnpoint)
        {

        }

        public override void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed)
        {

        }

        public void InitPath(GridNode[] path)
        {
            List<GridNode> gridNodes = new List<GridNode>(path);
            gridNodes.Reverse();
            this.path = gridNodes.ToArray();
        }
    }
}