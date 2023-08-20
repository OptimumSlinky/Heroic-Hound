using UnityEngine;

// Basic camera that follows the player character
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothTime = 0.25f;
    private Vector3 _offset;
    private Vector3 _currentVelocity = Vector3.zero;

    // Set initial offset
    void Awake()
    {
        _offset = transform.position - _player.position;
    }

    // Update camera transform based on player position
    void LateUpdate()
    {
        Vector3 cameraTarget = _player.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, cameraTarget, ref _currentVelocity, _smoothTime);
    }
}