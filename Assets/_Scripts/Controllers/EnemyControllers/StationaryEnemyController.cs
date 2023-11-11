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
    private float turnSpeed = 20;

    private float fov = 90;
    private float detectionRange = 50f;

    private bool turningClockwise = true;

    private Quaternion startOrientation;
    private Quaternion endOrientation;

    private GameObject bulletPrefab;

    private float shootingCooldown = 0.5f;
    private float currentShootingCooldown = 0;

    private AIState aiState = AIState.LookingForPlayer;

    public void Setup(Transform startAngleIndicator, Transform endAngleIndicator)
    {
        startAngle = startAngleIndicator.eulerAngles.z % 360;
        startOrientation = Utility.OrientationFromAngle(startAngle);
        endAngle = endAngleIndicator.eulerAngles.z % 360;
        endOrientation = Utility.OrientationFromAngle(endAngle);



        //Debug.Log("Start angle: " + startAngle + " End angle: " + endAngle + " Abs angle range: " + absAngleRange);

        transform.rotation = startOrientation;
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
        TickShootingCooldown();

        switch (aiState)
        {
            case AIState.LookingForPlayer:
                LookingForPlayer();
                break;
            case AIState.TracingPlayer:
                TracingPlayer();
                break;
            case AIState.ReturningToScanArea:
                ReturningToScanArea();
                break;
        }
    }

    private void LookingForPlayer()
    {
        Vector3 playerPosition = LevelManager.Instance.player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position,transform.up, playerPosition, detectionRange, fov))
        {
            ChangeAIState(AIState.TracingPlayer);
        }

        if (turningClockwise)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, endOrientation, turnSpeed * Time.fixedDeltaTime);
            if (transform.rotation == endOrientation)
            {
                turningClockwise = false;
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startOrientation, turnSpeed * Time.fixedDeltaTime);
            if (transform.rotation == startOrientation)
            {
                turningClockwise = true;
            }
        }
    }

    private void TracingPlayer()
    {
        Vector3 playerPosition = LevelManager.Instance.player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position, transform.up, playerPosition, detectionRange, fov))
        {
            Vector3 playerFacingDirection = playerPosition - transform.position;
            Quaternion orientationTowardsPlayer = Utility.OrientationFromVector3(playerFacingDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, orientationTowardsPlayer, turnSpeed * Time.fixedDeltaTime);
            
            Shooting();
        }
        else
        {
            ChangeAIState(AIState.ReturningToScanArea);
        }
    }

    private void ReturningToScanArea()
    {
        Vector3 playerPosition = LevelManager.Instance.player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position, transform.up, playerPosition, detectionRange, fov))
        {
            ChangeAIState(AIState.TracingPlayer);
        }

        float angleTowardsStartOrientation = Quaternion.Angle(transform.rotation, startOrientation);
        float angleTowardsEndOrientation = Quaternion.Angle(transform.rotation, endOrientation);

        Quaternion targetOrientation = angleTowardsStartOrientation > angleTowardsEndOrientation ? endOrientation : startOrientation;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetOrientation, turnSpeed * Time.fixedDeltaTime);

        if (transform.rotation == targetOrientation)
        {
            ChangeAIState(AIState.LookingForPlayer);
        }
    }

    private void ChangeAIState(AIState newState)
    {
        Debug.Log($"Changing from state {aiState} to state {newState}");
        aiState = newState;

        // Execute immediate state dependent changes here
    }

    private void Shooting()
    {
        if (currentShootingCooldown == 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.layer = LayerMask.NameToLayer("Enemies");
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.collisionLayers = LayerMask.GetMask("Environment", "Player");

            currentShootingCooldown = shootingCooldown;
        }
    }

    private void TickShootingCooldown()
    {
        currentShootingCooldown = Mathf.Clamp(currentShootingCooldown - Time.deltaTime, 0, shootingCooldown);
    }

    private enum AIState
    {
        LookingForPlayer,
        TracingPlayer,
        ReturningToScanArea
    }
}
