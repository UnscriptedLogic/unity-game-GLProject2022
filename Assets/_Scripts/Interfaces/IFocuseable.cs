using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityBehaviours;

namespace Interfaces
{
    public interface IFocuseable
    {
        void FocusObject(Transform partToRotate, Vector3 target, float rotationSpeed);
    }
}