using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime;
    
    private Vector3 _offset;
    private Vector3 _targetPosition;
    private Vector3 _currentVelocity = Vector3.zero;

    private void Awake()
    {
        _offset = transform.position - _target.position;
    }

    private void FixedUpdate()
    {
        _targetPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, _smoothTime);
    }
}
