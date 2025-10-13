using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathMover
{
    private Transform _transform;
    private Path _path;
    private Vector3 _targetPosition;
    private Transform _targetPointTransform;
    private float _speed;
    private float _offset;
    private Vector3 _offsetVector;
    private bool _isReachEndPoint = false;

    public event Action ReachedEndPoint;

    public PathMover(Transform transform, float speed, Path path, float offset = 0.1f)
    {
        _speed = speed;
        _transform = transform;
        _offset = offset;
        _offsetVector = new Vector3(Random.Range(-_offset, _offset), 0, 0);

        SetPath(path);
    }

    public void SetPath(Path path)
    {
        _path = path;
        _targetPointTransform = _path.GetCloserPoint(_transform.position);
        _targetPosition = _targetPointTransform.position;
        _isReachEndPoint = false;

        ConculateTargetPointWithOffset();
    }

    public void Restart()
    {
        _isReachEndPoint = false;
        _transform.position = _path.Points[0].position;
        _targetPointTransform = _path.Points[0];
        _targetPosition = _targetPointTransform.position;

        ConculateTargetPointWithOffset();
    }

    public void Update()
    {
        if (_path == null || _isReachEndPoint)
            return;

        _transform.LookAt(_targetPosition);
        _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, Time.deltaTime * _speed);

        if (Vector3.Distance(_transform.position, _targetPosition) <= _offset)
        {
            _targetPointTransform = _path.GetNextPoint(_targetPointTransform);

            if (_targetPointTransform == null)
            {
                _isReachEndPoint = true;
                ReachedEndPoint?.Invoke();
                return;
            }
            ConculateTargetPointWithOffset();
        }
    }

    private void ConculateTargetPointWithOffset()
    {
        Vector3 targetPositionWithOffset = _transform.InverseTransformPoint(_targetPointTransform.position);
        targetPositionWithOffset += _offsetVector;
        targetPositionWithOffset = _transform.TransformPoint(targetPositionWithOffset);

        _targetPosition = targetPositionWithOffset;
    }
}
