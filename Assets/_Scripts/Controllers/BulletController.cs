using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Transform bulletTransform;
    public Vector3 flightDirection;
    public LayerMask collisionLayers;

    private float speed = 40;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = bulletTransform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers.value) > 0)
        {
            if (((1 << collision.collider.gameObject.layer) & LayerMask.GetMask("Player", "Enemies")) > 0)
            {
                collision.collider.gameObject.SetActive(false);

                if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") && collision.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Player hit");
                    Debug.Log(collision.collider.name + " is colliding with " + collision.otherCollider.name);
                    LevelManager.Instance.ChangeState(GameState.Dead);
                }

                if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemies") && collision.collider.gameObject.tag == "Enemy")
                {
                    LevelManager.Instance.CheckWinCondition();
                }
            }

            Destroy(bulletTransform.gameObject);
        }
    }
}
