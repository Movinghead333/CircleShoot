using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject Player;

    public List<GameObject> AllEnemies = new();

    public void RegisterPlayer(GameObject playerGameobject)
    {
        Player = playerGameobject;
    }

    public void RegisterEnemy(GameObject enemyGameobject)
    {
        AllEnemies.Add(enemyGameobject);
    }

    public void Clear()
    {
        Player = null;
        AllEnemies.Clear();
    }
}
