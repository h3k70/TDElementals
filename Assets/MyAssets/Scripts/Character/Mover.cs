using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mover
{
    private Transform _transform;
    private Vector3 _targetPosition;
    private float _speed;
    private float _offset;
    private Vector3 _offsetVector;
    private bool _isReachEndPoint = false;

    public event Action ReachedEndPoint;

    public Mover(Transform transform, float speed, float offset = 0.1f)
    {
        _speed = speed;
        _transform = transform;
        _offset = offset;
        _offsetVector = new Vector3(Random.Range(-_offset, _offset), 0, 0);
    }

    public void SetPoint(Vector3 point)
    {
        _targetPosition = point;
        ConculateTargetPointWithOffset();
        _isReachEndPoint = false;
    }

    public void Update()
    {
        if (_isReachEndPoint)
            return;

        _transform.LookAt(_targetPosition);
        _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, Time.deltaTime * _speed);

        if (Vector3.Distance(_transform.position, _targetPosition) <= _offset)
        {
            ReachedEndPoint?.Invoke();
            _isReachEndPoint = true;
        }
    }

    private void ConculateTargetPointWithOffset()
    {
        Vector3 targetPositionWithOffset = _transform.InverseTransformPoint(_targetPosition);
        targetPositionWithOffset += _offsetVector;
        targetPositionWithOffset = _transform.TransformPoint(targetPositionWithOffset);

        _targetPosition = targetPositionWithOffset;
    }
}
