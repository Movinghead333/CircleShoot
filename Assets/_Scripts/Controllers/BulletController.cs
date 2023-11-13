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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers.value) > 0)
        {
            if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Player", "Enemies")) > 0)
            {
                collision.gameObject.SetActive(false);

                if (collision.gameObject.tag == "Player")
                {
                    Debug.Log("Player hit");
                    Debug.Log("Bullet is colliding with " + collision.gameObject.name);
                    LevelManager.Instance.ChangeState(GameState.Dead);
                }

                if (collision.gameObject.tag == "Enemy")
                {
                    LevelManager.Instance.CheckWinCondition();
                }
            }

            Destroy(bulletTransform.gameObject);
        }
    }
}
