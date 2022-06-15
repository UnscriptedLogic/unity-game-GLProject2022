using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Grid;
using Core.Pathing;

namespace Game
{
    public class MultiplayerLevelManager : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [SerializeField] private GameObject gameSet;
        [SerializeField] private Transform[] playerPositions;

        private void Start()
        {
            mapManager.RandomSeed();
            StartCoroutine(CreateMaps());
        }

        private void CreateSet(Transform anchor)
        {
            GameObject set = Instantiate(gameSet, anchor.position, Quaternion.identity);
            mapManager.GenerateMap(parent: set.transform, set.GetComponent<PathManager>());
        }

        private IEnumerator CreateMaps()
        {
            for (int i = 0; i < playerPositions.Length; i++)
            {
                CreateSet(playerPositions[i]);
                yield return new WaitForSeconds(3f);
            }
        }
    }
}