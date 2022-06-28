using Core.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ClickToggleUIButton : MonoBehaviour
    {
        protected Button button;
        [SerializeField] protected TextMeshProUGUI buttonText;
        protected int currentIndex = 0;

        protected virtual void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnToggle);
        }

        protected virtual void OnToggle()
        {
            currentIndex++;
        }
    }
}