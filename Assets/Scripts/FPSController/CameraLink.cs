using UnityEngine;

class CameraLink : MonoBehaviour
{
    public static Camera Instance;

    private void Awake()
    {
        Instance = GetComponent<Camera>();
    }
}
