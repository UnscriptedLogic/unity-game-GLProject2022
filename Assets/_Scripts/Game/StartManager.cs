using Core.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;

    private void Start()
    {
        gameSceneManager.HideLoading();
    }
}
