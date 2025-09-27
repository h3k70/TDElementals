using System;
using UnityEngine;
using UnityEngine.AI;

public class AgentMover
{
    private NavMeshAgent _agent;
    private Path _path;
    private bool _isMoving = false;
    private bool _isReachEndPoint = false;
    private Transform _currentTransform;
    private Vector3 _currentPoint;
    private float _offset = 1f;

    public event Action ReachedEndPoint;

    public AgentMover(NavMeshAgent agent, float speed, Path path)
    {
        _agent = agent;
        _agent.speed = speed;
        _path = path;
    }

    public void Update()
    {
        if (_isReachEndPoint)
            return;
        
        if (_isMoving == false)
        {
            _currentTransform = _path.GetNextPoint(_path.GetCloserPoint(_agent.transform.position));

            if (_currentTransform != null)
            {
                _currentPoint = _currentTransform.position;
                _agent.SetDestination(_currentPoint);
                _isMoving = true;
            }
            else
            {
                ReachedEndPoint?.Invoke();
                _isReachEndPoint = true;
            }
        }
        if (Vector3.Distance(_agent.transform.position, _currentPoint) <= _offset)
        {
            _isMoving = false;
        }
    }

    public void Pause()
    {
        _agent.isStopped = true;
    }

    public void Resume()
    {
        _agent.isStopped = false;
    }
}
