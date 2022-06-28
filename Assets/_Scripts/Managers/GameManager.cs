using Core.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class GameManager
    {
        //General
        public static string deviceID;
        public static string playfabID;
        public static bool isAnonymous;
        public static bool loggedIn;
        public static string username = null;
        public static int gamesPlayed;
        public static bool hasSeenThanksPage;

        //Game Stats
        public static int seed;
        public static bool setSeed;
        public static int wins = 0;
        public static int loss = 0;
        public static int highestWave = 0;
        public static int lowestTowerUsage = 100;

        //Game Related
        public static ThemeSO themeSO;
    }
}