using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Game.Spawning
{
    [CreateAssetMenu(fileName = "WaveFormat", menuName = "ScriptableObjects/Create Wave Object")]
    public class WavesSO : ScriptableObject
    {
        [SerializeField] private Wave[] waves;
        public Wave[] Waves => waves;

        private void OnValidate()
        {
            for (int i = 0; i < waves.Length; i++)
            {
                float points = 0f;
                float combinedHealth = 0f;
                float combinedSpeed = 0f;
                for (int j = 0; j < waves[i].waveSegments.Length; j++)
                {
                    Unit unit = waves[i].waveSegments[j].enemyToSpawn.GetComponent<Unit>();
                    points += unit.MaxHealth * waves[i].waveSegments[j].amount;
                    combinedHealth += unit.MaxHealth;
                    combinedSpeed += unit.Speed;
                }

                waves[i].totalPoints = points;
                waves[i].averageHealth = combinedHealth / waves[i].waveSegments.Length;
                waves[i].averageSpeed = combinedSpeed / waves[i].waveSegments.Length;
            }
        }
    }

    [Serializable]
    public class WaveSegment
    {
        public GameObject enemyToSpawn;
        public int amount;
        public float interval;
        public float segmentInterval;
    }

    [Serializable]
    public class Wave
    {
        public WaveSegment[] waveSegments;
        public float waveInterval = 5f;
 
        [Header("Stats")]
        public float totalPoints;
        public float averageHealth;
        public float averageSpeed;
    }
}
