using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading.Tasks;
using Core;

namespace Backend
{
    public static class PlayFabManager
    {
        public const string WINS = "wins";
        public const string LOSS = "losses";
        public const string HIGHEST_WAVE = "highestWave";
        public const string GAMES_PLAYED = "gamesPlayed";
        public const string SEEN_TY_PAGE = "seenThanksPage";
        public const string DEVICE_ID = "DeviceID";

        public static void AnonymousLogIn(Action<LoginResult> onSuccess, Action<PlayFabError> onError)
        {
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
            {
                CustomId = PlayerPrefs.GetString(DEVICE_ID),
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithCustomID(request, success =>
            {
                onSuccess(success);
                GameManager.loggedIn = true;
                GameManager.playfabID = success.PlayFabId;

                if (success.InfoResultPayload.PlayerProfile != null)
                    GameManager.username = success.InfoResultPayload.PlayerProfile.DisplayName;

            }, onError);
        }



        #region Tower Count
        public static void SendLeaderboardTowerCount(int towerCount)
        {
            UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "GameBeatenWithTowerCount",
                        Value = towerCount
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, (result) => { Debug.Log($"Error! {result.GenerateErrorReport() }"); });
        }

        public static void GetTowerCountScores(Action<GetLeaderboardResult> OnSuccess, Action<PlayFabError> OnError)
        {
            GetLeaderboardRequest request = new GetLeaderboardRequest
            {
                StatisticName = "GameBeatenWithTowerCount",
                StartPosition = 0,
                MaxResultsCount = 10
            };

            PlayFabClientAPI.GetLeaderboard(request, OnSuccess, OnError);
        }

        #endregion

        #region HighestWave
        public static void SendHighestWaveCount(int waveCount)
        {
            UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "highest_wave",
                        Value = waveCount
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, (result) => { Debug.Log($"Error! {result.GenerateErrorReport() }"); });
        }

        public static void GetWaveScores(Action<GetLeaderboardResult> OnSuccess, Action<PlayFabError> OnError)
        {
            GetLeaderboardRequest request = new GetLeaderboardRequest
            {
                StatisticName = "highest_wave",
                StartPosition = 0,
                MaxResultsCount = 10
            };

            PlayFabClientAPI.GetLeaderboard(request, OnSuccess, OnError);
        }
        #endregion

        #region Player Data
        public static void SavePlayerData(Action<UpdateUserDataResult> OnSuccess, Action<PlayFabError> OnError)
        {
            UpdateUserDataRequest request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    {WINS, GameManager.wins.ToString() },
                    {LOSS, GameManager.loss.ToString() },
                    {GAMES_PLAYED, GameManager.gamesPlayed.ToString() },
                    {HIGHEST_WAVE, GameManager.highestWave.ToString() },
                    {SEEN_TY_PAGE, GameManager.hasSeenThanksPage.ToString() }
                }
            };

            PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnError);
        }

        public static void GetPlayerData(Action<GetUserDataResult> onDataRecieved, Action<PlayFabError> onError)
        {
            Debug.Log("Getting player data");
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
            {
                if (GameManager.loggedIn)
                {
                    if (result.Data != null)
                    {
                        if (result.Data.ContainsKey(WINS))
                        {
                            GameManager.wins = int.Parse(result.Data[WINS].Value);
                        }

                        if (result.Data.ContainsKey(LOSS))
                        {
                            GameManager.loss = int.Parse(result.Data[LOSS].Value);
                        }

                        if (result.Data.ContainsKey(HIGHEST_WAVE))
                        {
                            GameManager.highestWave = int.Parse(result.Data[HIGHEST_WAVE].Value);
                        }

                        if (result.Data.ContainsKey(GAMES_PLAYED))
                        {
                            GameManager.gamesPlayed = int.Parse(result.Data[GAMES_PLAYED].Value);
                        }

                        if (result.Data.ContainsKey(SEEN_TY_PAGE))
                        {
                            GameManager.hasSeenThanksPage = bool.Parse(result.Data[SEEN_TY_PAGE].Value);
                        }
                    }
                }

                onDataRecieved(result);
            }, onError);
        }

        public static void UpdateUsername(string username, Action<UpdateUserTitleDisplayNameResult> onSuccess, Action<PlayFabError> onError)
        {
            UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = username
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, onSuccess, onError);
        } 
        #endregion

        private static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Successfully updated the leaderboard");
        }
        public static void HandleError(PlayFabError playFabError)
        {
            Debug.LogWarning($"ERROR! {playFabError.GenerateErrorReport()}");
        }
    }
}