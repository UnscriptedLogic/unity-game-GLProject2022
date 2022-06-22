using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Assets
{

    [CreateAssetMenu(fileName = "Game Theme", menuName = "ScriptableObjects/New Game Theme")]
    public class ThemeSO : ScriptableObject
    {
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject homePrefab;
        [SerializeField] private GameObject spawnPrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject[] debriList;

        public GameObject Node => nodePrefab;
        public GameObject Home => homePrefab;
        public GameObject Spawn => spawnPrefab;
        public GameObject Floor => floorPrefab;
        public GameObject[] DebriList => debriList;
    }
}