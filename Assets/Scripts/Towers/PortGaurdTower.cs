using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class PortGaurdTower : Tower
    {
        [Header("PortGaurd Extension")]
        [SerializeField] private Transform leftShootAnchor;
        [SerializeField] private Transform rightShootAnchor;

        private bool left;

        public override void DoAttack(GameObject projectile, Transform spawnpoint)
        {
            if (left)
            {
                base.DoAttack(projectile, leftShootAnchor);
                left = false;
            }
            else
            {
                base.DoAttack(projectile, rightShootAnchor);
                left = true;
            }
        }

        public override void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed)
        {
            base.FocusObject(partToRotate, target, rotationSpeed);
        }
    }
}