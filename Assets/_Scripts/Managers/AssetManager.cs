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

        [SerializeField] private GameObject testTower;
        [SerializeField] private GameObject worldSpaceSlider;
        [SerializeField] private GameObject placedParticle;

        public GameObject Tower { get => testTower; }
        public GameObject WorldSpaceSlider { get => worldSpaceSlider; }
        public GameObject PlacedParticle { get => placedParticle; }
    }
}