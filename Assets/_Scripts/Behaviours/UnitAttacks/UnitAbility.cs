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
        
        protected GreaterUnit context;
        protected AbilityState currentState;

        public virtual void Initialize(GreaterUnit _context)
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