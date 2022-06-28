using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Assets
{
    public class AssetManager : MonoBehaviour
    {
        public static AssetManager instance;
        private void Awake() => instance = this;

        [SerializeField] private ThemeSO themeFile;

        [SerializeField] private GameObject placedParticle;
        [SerializeField] private GameObject flameParticle;

        public GameObject PlacedParticle { get => placedParticle; }
        public GameObject FlameParticle { get => flameParticle; }
        public ThemeSO ThemeFile { get => themeFile; set { themeFile = value; } }
    }
}