using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform { private get; set; }

    private Vector3 _offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (PlayerTransform != null)
        {
            transform.position = PlayerTransform.position + _offset;
        }
    }
}

