using UnityEngine;

class CameraControl : MonoBehaviour
{
    public static Camera Instance;
    public static float LookSpeedH = 2f;
    public static float LookSpeedV = 2f;

    float _yaw;
    float _pitch;

    private void Start()
    {
        Instance = GetComponent<Camera>();
        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        _yaw += LookSpeedH * Input.GetAxis("Mouse X");
        _pitch -= LookSpeedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}
