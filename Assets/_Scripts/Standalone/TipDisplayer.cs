using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Standalone
{
    public class TipDisplayer : MonoBehaviour
    {
        [SerializeField] private TipSO tipSO;
        [SerializeField] private TextMeshProUGUI tipTMP;

        private static string tip = "You can regenerate the map in the pause menu at the top left!";

        public void DisplayTip()
        {
            tipTMP.text = tip;
        }

        public void GenerateTip()
        {
            tip = MathHelper.RandomFromList(tipSO.Tips);
        }
    }
}