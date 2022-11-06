using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class PlayFabBehaviour : MonoBehaviour
    {
        [SerializeField] private bool SelfSignIn;

        private void Start()
        {
            if (SelfSignIn)
                PlayFabManager.AnonymousLogIn(success => { Debug.Log("Success"); }, error => { Debug.Log($"Error! {error.GenerateErrorReport()}"); });
        }

        public void UpdateLeaderboardTowerCount(int count) => PlayFabManager.SendLeaderboardTowerCount(count);
        public void HighestWaveCount(int count) { PlayFabManager.SendHighestWaveCount(count); }
    }
}