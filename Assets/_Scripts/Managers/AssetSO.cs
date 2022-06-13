using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Assets
{

    [CreateAssetMenu(fileName = "Asset", menuName = "ScriptableObjects/New Asset File")]
    public class AssetSO : ScriptableObject
    {
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject homePrefab;
        [SerializeField] private GameObject spawnPrefab;
        [SerializeField] private GameObject placedParticle;
        [SerializeField] private GameObject[] debris;

        public GameObject NodePrefab => nodePrefab;
        public GameObject HomePrefab => homePrefab;
        public GameObject SpawnPrefab => spawnPrefab;
        public GameObject PlacedParticle => placedParticle;
        public GameObject[] DebrisList => debris;
    }
}