using Core;
using Core.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ThemeToggleButton : ClickToggleUIButton
    {
        [Serializable]
        public class ThemeOptions
        {
            public string label = "Random";
            public ThemeSO themeSO;
        }

        [SerializeField] private ThemeOptions[] themeOptions;

        protected override void Start()
        {
            base.Start();
            buttonText.text = themeOptions[currentIndex].label;
        }

        protected override void OnToggle()
        {
            currentIndex = currentIndex == themeOptions.Length - 1 ? 0 : currentIndex+1;
            buttonText.text = themeOptions[currentIndex].label;
            GameManager.themeSO = GetCurrentOption();
        }

        public ThemeSO GetCurrentOption()
        {
            if (themeOptions[currentIndex].label.ToLower() == "random")
            {
                ThemeSO themeSO = null;
                while (themeSO == null)
                {
                    themeSO = MathHelper.RandomFromArray(themeOptions).themeSO;
                }

                return themeSO;
            }
            else
            {
                return themeOptions[currentIndex].themeSO;
            }
        }
    }
}