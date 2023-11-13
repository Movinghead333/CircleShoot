using UnityEngine;

public class StationaryEnemyController : MonoBehaviour
{
    [SerializeField] public bool DrawGismos = false;

    private float _startAngle = 0;
    private float _endAngle = 0;
    private float _turnSpeed = 15;

    private float _fov = 45;
    private float _detectionRange = 50f;

    private bool _turningClockwise = true;

    private Quaternion _startOrientation;
    private Quaternion _endOrientation;

    private GameObject _bulletPrefab;

    private float _shootingCooldown = 0.5f;
    private float _currentShootingCooldown = 0;

    private AIState _aiState = AIState.LookingForPlayer;

    public void Setup(Transform startAngleIndicator, Transform endAngleIndicator)
    {
        _startAngle = startAngleIndicator.eulerAngles.z % 360;
        _startOrientation = Utility.OrientationFromAngle(_startAngle);
        _endAngle = endAngleIndicator.eulerAngles.z % 360;
        _endOrientation = Utility.OrientationFromAngle(_endAngle);

        transform.rotation = _startOrientation;
    }

    private void OnDrawGizmos()
    {
        Vector3 leftFOVLimit = transform.position + (Quaternion.Euler(0,0, _fov/2f) * transform.up) * _detectionRange;
        Vector3 rightFOVLimit = transform.position + (Quaternion.Euler(0, 0, -_fov / 2f) * transform.up) * _detectionRange;


        Gizmos.color = new Color(0f, 1f, 0f);
        Vector3 playerPosition = LevelManager.Instance.Player.transform.position;
        bool playerIsInDetectionRange = Utility.IsInRange(transform.position, playerPosition, _detectionRange);
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

        float arcLength = _detectionRange * 2f * Mathf.PI * (_fov / 360f);
        int arcSegments = (int)arcLength;

        Vector3 from = rightFOVLimit;
        Vector3 to;
        for (int i = 0; i < arcSegments; i++)
        {
            float angleOffset = _fov * ((float)(i+1) / (arcSegments+1));
            to = transform.position + (Quaternion.Euler(0, 0, (-_fov / 2f) + angleOffset) * transform.up) * _detectionRange;

            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.DrawLine(from, leftFOVLimit);
    }

    // Start is called before the first frame update
    void Start()
    {
        _bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");

        LevelManager.Instance.RegisterEnemy(gameObject);
    }

    private void Update()
    {
        TickShootingCooldown();

        switch (_aiState)
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
        Vector3 playerPosition = LevelManager.Instance.Player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position,transform.up, playerPosition, _detectionRange, _fov))
        {
            ChangeAIState(AIState.TracingPlayer);
        }

        if (_turningClockwise)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _endOrientation, _turnSpeed * Time.fixedDeltaTime);
            if (transform.rotation == _endOrientation)
            {
                _turningClockwise = false;
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _startOrientation, _turnSpeed * Time.fixedDeltaTime);
            if (transform.rotation == _startOrientation)
            {
                _turningClockwise = true;
            }
        }
    }

    private void TracingPlayer()
    {
        Vector3 playerPosition = LevelManager.Instance.Player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position, transform.up, playerPosition, _detectionRange, _fov))
        {
            Vector3 playerFacingDirection = playerPosition - transform.position;
            Quaternion orientationTowardsPlayer = Utility.OrientationFromVector3(playerFacingDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, orientationTowardsPlayer, _turnSpeed * Time.fixedDeltaTime);
            
            Shooting();
        }
        else
        {
            ChangeAIState(AIState.ReturningToScanArea);
        }
    }

    private void ReturningToScanArea()
    {
        Vector3 playerPosition = LevelManager.Instance.Player.transform.position;
        if (Utility.IsInFOVConeAndLineOfSightOptimized(transform.position, transform.up, playerPosition, _detectionRange, _fov))
        {
            ChangeAIState(AIState.TracingPlayer);
        }

        float angleTowardsStartOrientation = Quaternion.Angle(transform.rotation, _startOrientation);
        float angleTowardsEndOrientation = Quaternion.Angle(transform.rotation, _endOrientation);

        Quaternion targetOrientation = angleTowardsStartOrientation > angleTowardsEndOrientation ? _endOrientation : _startOrientation;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetOrientation, _turnSpeed * Time.fixedDeltaTime);

        if (transform.rotation == targetOrientation)
        {
            ChangeAIState(AIState.LookingForPlayer);
        }
    }

    private void ChangeAIState(AIState newState)
    {
        Debug.Log($"Changing from state {_aiState} to state {newState}");
        _aiState = newState;

        // Execute immediate state dependent changes here
    }

    private void Shooting()
    {
        if (_currentShootingCooldown == 0)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation);
            bullet.layer = LayerMask.NameToLayer("Enemies");
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.collisionLayers = LayerMask.GetMask("Environment", "Player");

            _currentShootingCooldown = _shootingCooldown;
        }
    }

    private void TickShootingCooldown()
    {
        _currentShootingCooldown = Mathf.Clamp(_currentShootingCooldown - Time.deltaTime, 0, _shootingCooldown);
    }

    private enum AIState
    {
        LookingForPlayer,
        TracingPlayer,
        ReturningToScanArea
    }
}
