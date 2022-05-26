using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityBehaviours
{
    public class LookBehaviour
    {
        public void FocusTarget(Transform partToRotate, Vector3 targetPosition, float rotationSpeed)
        {
            //Quaternion currentRotation = partToRotate.rotation;
            //Quaternion targetRotation = Quaternion.LookRotation(targetPosition);
            //partToRotate.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);

            Vector3 dir = targetPosition - partToRotate.transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRot, rotationSpeed * Time.deltaTime).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }
}