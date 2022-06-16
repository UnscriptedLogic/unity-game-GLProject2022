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

    public float AddWaveIncome(int waveCount)
    {
        float evaluation;
        if (waveCount <= waveCap)
            evaluation = waveCount / (float)waveCap;
        else
            evaluation = waveCap;

        return (float)Mathf.Round(animationCurve.Evaluate(evaluation) * multiplier);
    }
}
