using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyController : MonoBehaviour
{
    //[SerializeField]
    private float startAngle = 0;
    //[SerializeField]
    private float endAngle = 0;
    private float turnSpeed = 50;

    [SerializeField]
    private float absAngleRange;
    private float currentAngle;
    private bool turningClockwise = true;

    private GameObject bulletPrefab;

    private float shootingCooldown = 0.5f;
    private float currentShootingCooldown = 0;

    public void Setup(Transform startAngleIndicator, Transform endAngleIndicator)
    {
        startAngle = startAngleIndicator.eulerAngles.z % 360;
        endAngle = endAngleIndicator.eulerAngles.z % 360;

        if (startAngle > endAngle)
        {
            absAngleRange = startAngle - endAngle;
        }
        else
        {
            absAngleRange = 360f - (endAngle - startAngle);
        }

        //Debug.Log("Start angle: " + startAngle + " End angle: " + endAngle + " Abs angle range: " + absAngleRange);

        transform.rotation = Quaternion.Euler(0, 0, startAngle);
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");

        LevelManager.Instance.RegisterEnemy(gameObject);
    }

    private void Update()
    {
        Shooting();
    }

    private void FixedUpdate()
    {
        if (turningClockwise)
        {
            if (currentAngle < absAngleRange)
            {
                currentAngle += turnSpeed * Time.fixedDeltaTime;
            }
            else
            {
                currentAngle = absAngleRange;
                turningClockwise = false;
            }
        }
        else
        {
            if (currentAngle > 0)
            {
                currentAngle -= turnSpeed * Time.fixedDeltaTime;
            }
            else
            {
                currentAngle = 0;
                turningClockwise = true;
            }
        }

        float angle = (startAngle - currentAngle) % 360;

        transform.rotation = Quaternion.Euler(0, 0, angle);
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
