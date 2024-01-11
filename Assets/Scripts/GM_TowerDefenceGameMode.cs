using System.Collections;
using UnityEngine;
using UnscriptedEngine;

public class GM_TowerDefenceGameMode : UGameModeBase
{
    [SerializeField] private MapGenerator mapGenerator;

    protected override IEnumerator Start()
    {
        yield return base.Start();

        mapGenerator.GenerateMap();
    }
}