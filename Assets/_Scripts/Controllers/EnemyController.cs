using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float startAngle = 45;
    public float endAngle = 315;
    public float turnSpeed = 5;

    public float currentAngle;
    private bool turningUp = true;

    private GameObject bulletPrefab;

    private float shootingCooldown = 0.5f;
    private float currentShootingCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        currentAngle = startAngle;
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        LevelManager.Instance.RegisterEnemy(gameObject);
    }

    private void Update()
    {
        Shooting();
    }

    private void FixedUpdate()
    {
        if (turningUp)
        {
            if (currentAngle < endAngle)
            {
                currentAngle += turnSpeed * Time.fixedDeltaTime;
            } else
            {
                currentAngle = endAngle;
                turningUp = false;
            }
        }
        else
        {
            if (currentAngle > startAngle)
            {
                currentAngle -= turnSpeed * Time.fixedDeltaTime;
            }
            else
            {
                currentAngle = startAngle;
                turningUp = true;
            }
        }

        currentAngle = (currentAngle + 360f) % 360f;

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    private void Shooting()
    {
        if (currentShootingCooldown > 0)
        {
            currentShootingCooldown = Mathf.Clamp(currentShootingCooldown - Time.deltaTime, 0, shootingCooldown);
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.layer = LayerMask.NameToLayer("Enemies");
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.collisionLayers = LayerMask.GetMask("Environment", "Player");

            currentShootingCooldown = shootingCooldown;
        }
    }
}
