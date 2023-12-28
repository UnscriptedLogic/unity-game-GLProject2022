using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;
using UnscriptedEngine;
using System.Collections;

public class GM_TowerDefenceGameMode : UGameModeBase
{
    [SerializeField] private MapGenerator mapGenerator;

    protected override IEnumerator Start()
    {
        yield return base.Start();

        mapGenerator.GenerateMap();
    }
}