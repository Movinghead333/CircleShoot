using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemySpawner : MonoBehaviour
{
    public Transform startAngleIndicator;
    public Transform endAngleIndicator;

    private void Start()
    {
        GameObject stationaryEnemyPrefab = Resources.Load<GameObject>("Prefabs/StationaryEnemy");
        GameObject environment = GameObject.Find("Environment");
        GameObject stationaryEnemyGameObject = Instantiate(stationaryEnemyPrefab, transform.position, Quaternion.identity, environment.transform);
        stationaryEnemyGameObject.GetComponent<StationaryEnemyController>().Setup(startAngleIndicator, endAngleIndicator);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "enemy_gizmo.png", true, Color.red);
    }
}
