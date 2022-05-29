using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.CustomEventSystem
{
    public interface IVoidEventStarter
    {
        void CheckEvent();
        void FireEvent();
    }
}