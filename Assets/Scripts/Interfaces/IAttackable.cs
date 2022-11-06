using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IAttackable
    {
        void DoAttack(GameObject projectile, Transform spawnpoint);
    }
}