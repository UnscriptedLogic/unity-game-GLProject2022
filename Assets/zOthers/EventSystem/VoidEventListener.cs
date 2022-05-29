using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.CustomEventSystem
{
    public class VoidEventListener : MonoBehaviour
    {
        public VoidEventSO eventSource;
        public UnityEvent OnEventRaised;

        private void Start()
        {
            if (eventSource != null)
                eventSource.OnEventRaised += () => OnEventRaised?.Invoke();
            else
                Debug.Log("Event Listener has no event referece!", gameObject);
        }
    }
}