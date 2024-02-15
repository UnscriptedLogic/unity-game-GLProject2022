using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine.BuildHandlers;

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

        public override void LocalPassBuildConditions<T>(T builder, out List<LocalBuildCondition> localBuildConditions)
        {
            localBuildConditions = new List<LocalBuildCondition>()
            {
                new LocalBuildCondition("Height Requirement", (position, rotation) => position.y >= 2f, "Not at the correct height", "At the correctHeight")
            };
        }
    }
}