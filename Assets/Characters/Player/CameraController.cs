using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothTime = 0.25f;
    private Vector3 _offset;
    private Vector3 _currentVelocity = Vector3.zero;

    void Awake()
    {
        _offset = transform.position - _player.position;
    }

    void LateUpdate()
    {
        Vector3 cameraTarget = _player.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, cameraTarget, ref _currentVelocity, _smoothTime);
    }
}