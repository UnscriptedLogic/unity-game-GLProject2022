using System.Collections;
using Core.Grid;
using Game.Spawning;
using UnityEngine;
using UnscriptedEngine;

public class GM_TowerDefenceGameMode : UGameModeBase
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private WaveSpawner waveSpawner;
    
    private GI_CustomGameInstance customGameInstance;
    
    protected override IEnumerator Start()
    {
        customGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();
        customGameInstance.LoadGame<SaveData>("DefaultMode");
        
        mapGenerator.GenerateMap();

        yield return base.Start();

        waveSpawner.Initialize(mapGenerator.Pathway.ToArray());
        waveSpawner.StartSpawner();
    }
}