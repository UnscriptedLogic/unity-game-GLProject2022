using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class UnitAbility : MonoBehaviour
    {
        protected enum AbilityState 
        {
            Ready,
            Casting,
            Done
        }
        
        protected EnemyUnits context;
        protected AbilityState currentState;

        public virtual void Initialize(EnemyUnits _context)
        {
            context = _context;
        }

        public virtual void EnterState()
        {

        }

        public virtual void UpdateState()
        {

        }

        public virtual void ExitState()
        {

        }
    }
}