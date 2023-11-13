#define NO_GAMEPAD_USED

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _speed = 10f;

    private float _shootingCooldown = 0.5f;
    private float _currentShootingCooldown = 0;

    private GameObject _bulletPrefab;

#if (GAMEPAD_USED)
    private Vector3 _lookDirection = Vector3.up;
#endif

    // Start is called before the first frame update
    void Start()
    {
        _bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        LevelManager.Instance.RegisterPlayer(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Shooting();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector3 inputVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputVector += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector += new Vector3(1, 0, 0);
        }

#if (GAMEPAD_USED)
        Vector2 gamepadLeftStickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (gamepadLeftStickInput != Vector2.zero)
        {
            inputVector = gamepadLeftStickInput;
        }
#endif

        inputVector.Normalize();
        transform.position += inputVector * _speed * Time.deltaTime;
    }

    private void RotatePlayer()
    {
#if (GAMEPAD_USED)
        Vector2 gamepadRightStickInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        Debug.Log("HOZ: " + gamepadRightStickInput.x + " VERT: " + gamepadRightStickInput.y + " MAG: " + gamepadRightStickInput.magnitude);

        if (gamepadRightStickInput.magnitude > 0.02)
        {
            _lookDirection = gamepadRightStickInput.normalized;
        }
#else
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 _lookDirection = mouseWorldPos - transform.position;
#endif
        float angle = Mathf.Atan2(-_lookDirection.x, _lookDirection.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shooting()
    {
        if (_currentShootingCooldown > 0)
        {
            _currentShootingCooldown = Mathf.Clamp(_currentShootingCooldown - Time.deltaTime, 0, _shootingCooldown);
        }
        else
        {
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.JoystickButton5)) {
                GameObject bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation);
                bullet.layer = LayerMask.NameToLayer("Player");
                BulletController bulletController = bullet.GetComponent<BulletController>();
                bulletController.collisionLayers = LayerMask.GetMask("Environment", "Enemies");

                _currentShootingCooldown = _shootingCooldown;
            }
        }
    }

    /*
     * PS5 controller mapping:
     * L1 -> JoystickButton4
     * 3-AX -> R-Stick X
     * 4-AX -> L2
     * 5-AX -> R2
     * 6-AX -> R-Stick Y
     */
}
