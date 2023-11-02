using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private float speed = 10f;

    private float shootingCooldown = 0.5f;
    private float currentShootingCooldown = 0;

    private GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
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

        inputVector.Normalize();
        transform.position += inputVector * speed * Time.deltaTime;
    }

    private void RotatePlayer()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 lookDir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(-lookDir.x, lookDir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shooting()
    {
        if (currentShootingCooldown > 0)
        {
            currentShootingCooldown = Mathf.Clamp(currentShootingCooldown - Time.deltaTime, 0, shootingCooldown);
        }
        else
        {
            if (Input.GetMouseButton(0)) {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                bullet.layer = LayerMask.NameToLayer("Player");
                BulletController bulletController = bullet.GetComponent<BulletController>();
                bulletController.collisionLayers = LayerMask.GetMask("Environment", "Enemies");

                currentShootingCooldown = shootingCooldown;
            }
        }
    }
}
