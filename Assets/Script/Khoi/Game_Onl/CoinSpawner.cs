using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CoinSpawner : SimulationBehaviour, ISceneLoadDone
{
    public NetworkPrefabRef coinPrefab;

    public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
    {
        var spawnPosition = new Vector3[]
        {
            new Vector3(Random.Range(-15f, -2f), 9, 0),
            new Vector3(Random.Range(-15f, -2f), 10, 0)
         
        };
        if (Runner.IsSharedModeMasterClient)
        {
            foreach (var position in spawnPosition)
            {
                Runner.Spawn(
                    coinPrefab,
                    position,
                    Quaternion.identity
                );
            }
        }
    }
}
