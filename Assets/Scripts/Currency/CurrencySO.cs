using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Currency
{
    [CreateAssetMenu(fileName = "Currency Container", menuName = "ScriptableObjects/Currency Container")]
    public class CurrencySO : ScriptableObject
    {
        [SerializeField] private float startAmount;
        [SerializeField] private Vector2Int clamp = new Vector2Int(0, 100);
        private float currAmount;

        public float CurrentAmount { get => currAmount; set { currAmount = value; } }
        public float StartAmount { get => startAmount; set { startAmount = value; } }

        public void CheckClampAmount() => currAmount = Mathf.Clamp(currAmount, clamp.x, clamp.y);
    }
}
