using System;
using System.Collections;
using UnityEngine;

public enum ModificationType
{
    Add,
    Subtract,
    Set,
    Divide,
    Multiply
}

namespace Interfaces
{
    public interface IDamageable
    {
        void ModifyHealth(ModificationType modificationType, float amount, float maximum = 0f, float minimum = 0f);
    }
}