using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyController : MonoBehaviour
{
    [SerializeField] public bool DrawGismos = false;

    //[SerializeField]
    private float startAngle = 0;
    //[SerializeField]
    private float endAngle = 0;
    private float turnSpeed = 50;

    private float fov = 90;
    private float detectionRange = 5f;

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

    private void OnDrawGizmos()
    {
        Vector3 leftFOVLimit = transform.position + (Quaternion.Euler(0,0, fov/2f) * transform.up) * detectionRange;
        Vector3 rightFOVLimit = transform.position + (Quaternion.Euler(0, 0, -fov / 2f) * transform.up) * detectionRange;


        Gizmos.color = new Color(0f, 1f, 0f);
        Vector3 playerPosition = LevelManager.Instance.player.transform.position;
        bool playerIsInDetectionRange = Utility.IsInRange(transform.position, playerPosition, detectionRange);
        if (playerIsInDetectionRange)
        {
            Gizmos.color = new Color(1f, 1f, 0f);
            bool isInFov = Utility.IsInFOVCone(transform.position, transform.up, 90, playerPosition);
            if (isInFov)
            {
                Gizmos.color = new Color(1f, 0.55f, 0f);
                bool playerInLineOfSight = Utility.IsInLineOfSight(transform.position, playerPosition, LayerMask.GetMask("Environment"));
                if (playerInLineOfSight)
                {
                    Gizmos.color = new Color(1f, 0f, 0f);
                }
            }
        }

        //Gizmos.color = new Color(1f, 0.55f, 0f);
        Gizmos.DrawLine(transform.position, leftFOVLimit);
        Gizmos.DrawLine(transform.position, rightFOVLimit);

        float arcLength = detectionRange * 2f * Mathf.PI * (fov / 360f);
        int arcSegments = (int)arcLength;

        Vector3 from = rightFOVLimit;
        Vector3 to;
        for (int i = 0; i < arcSegments; i++)
        {
            float angleOffset = fov * ((float)(i+1) / (arcSegments+1));
            to = transform.position + (Quaternion.Euler(0, 0, (-fov / 2f) + angleOffset) * transform.up) * detectionRange;

            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.DrawLine(from, leftFOVLimit);
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
            //GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            //bullet.layer = LayerMask.NameToLayer("Enemies");
            //BulletController bulletController = bullet.GetComponent<BulletController>();
            //bulletController.collisionLayers = LayerMask.GetMask("Environment", "Player");

            currentShootingCooldown = shootingCooldown;
        }
    }

}
