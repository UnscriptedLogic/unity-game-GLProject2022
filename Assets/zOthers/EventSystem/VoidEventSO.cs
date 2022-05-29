using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.CustomEventSystem
{

    [CreateAssetMenu(fileName = "VoidEvent", menuName = "ScriptableObjects/Events/Void Event")]
    public class VoidEventSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
                OnEventRaised?.Invoke(); 
        }
    }
}