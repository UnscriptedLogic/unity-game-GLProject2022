using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Currency;
using Game.Spawning;
using Game;

public class WaveIncome : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private int waveCap;
    [SerializeField] private float multiplier = 100f;

    private CurrencyManager currencyManager;
    private WaveSpawner waveSpawner;

    public void Initialize(LevelManager levelManager)
    {
        currencyManager = levelManager.CurrencyManager;
        waveSpawner = levelManager.WaveSpawner;

        waveSpawner.OnWaveCompleted += AddWaveIncome;
    }

    private void AddWaveIncome()
    {
        float evaluation;
        if (waveSpawner.WaveCount <= waveCap)
            evaluation = waveSpawner.WaveCount / (float)waveCap;
        else
            evaluation = waveCap;

        currencyManager.ModifyCurrency(ModificationType.Add, (float)Mathf.Round(animationCurve.Evaluate(evaluation) * multiplier));
    }
}
