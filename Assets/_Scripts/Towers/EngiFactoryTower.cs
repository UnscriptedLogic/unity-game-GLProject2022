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
        [SerializeField] private float tankSpawnInterval = 35f;
        [SerializeField] private float turretSpawnInterval = 20f;
        [SerializeField] private float towerLifeTime = 17f;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private GameObject turretPrefab;

        [SerializeField] private bool spawnTanks;
        [SerializeField] private bool spawnTurrets;
        [SerializeField] private LayerMask nodeLayer;

        private float _tankInterval;
        private float _turretInterval;

        private bool called;

        private PoolManager poolManager;
        private GridNode[] path;

        protected override void Start()
        {
            base.Start();
            poolManager = PoolManager.instance;
            _turretInterval = 5f;
        }

        protected override void Update()
        {
            if (spawnTanks)
            {
                if (_tankInterval <= 0f)
                {
                    GameObject unit = poolManager.PullFromPool(unitPrefab);
                    unit.GetComponent<Unit>().InitializeEnemy(path, 0);
                    _tankInterval = tankSpawnInterval;
                }
                else
                {
                    _tankInterval -= Time.deltaTime;
                } 
            }

            if (spawnTurrets)
            {
                if (_turretInterval <= 0f)
                {
                    if (!called)
                    {
                        bool invalid = true;
                        called = true;
                        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, range, nodeLayer));

                        while (invalid)
                        {
                            GridNode gridNode = GridGenerator.ParseGameObjectToNode(MathHelper.RandomFromList(colliders, out int index).gameObject.name);
                            if (!gridNode.isObstacle && !gridNode.IsOccupied && gridNode.isWithinElevation(Vector2.zero))
                            {
                                gridNode.PlaceTower(turretPrefab);
                                Destroy(gridNode.TowerOnNode, towerLifeTime);
                                _turretInterval = turretSpawnInterval;
                                invalid = false;
                                called = false;
                                Debug.Log("Hello", gridNode.TowerOnNode);
                                break;
                            }
                        } 
                    }
                }
                else
                {
                    _turretInterval -= Time.deltaTime;
                }
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