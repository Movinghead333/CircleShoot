using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        GameObject environment = GameObject.Find("Environment");
        GameObject playerGameObject = Instantiate(playerPrefab, transform.position, Quaternion.identity, environment.transform);

        Camera.main.gameObject.GetComponent<CameraController>().playerTransform = playerGameObject.transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "player_gizmo.png", true);
    }
}
