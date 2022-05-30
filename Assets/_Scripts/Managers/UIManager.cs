using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        public Action OnUpdateUI;

        public void SubscribeUpdateUI(Action method) => OnUpdateUI += method;
        public void FireUpdateUI() => OnUpdateUI?.Invoke();
    }
}
