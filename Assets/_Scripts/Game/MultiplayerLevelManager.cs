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
        [SerializeField] private Transform player1pos;
        [SerializeField] private Transform player2Pos;

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
            CreateSet(player1pos);
            yield return new WaitForSeconds(5f);
            CreateSet(player2Pos);
        }
    }
}