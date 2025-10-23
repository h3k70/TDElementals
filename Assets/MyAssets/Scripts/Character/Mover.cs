using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mover
{
    private NavMeshAgent _agent;
    private Character _character;
    private float _timeForDistanceReconculate = 0.2f;
    private Coroutine _moveCorounine;

    public bool IsMoveToTarget { get; private set; }

    public event Action ReachedEndPoint;

    public Mover(Character character)
    {
        _character = character;
        _agent = _character.Agent;
        _agent.speed = character.MoveSpeed;
    }

    public void MoveToTarget(Transform target, float offset)
    {
        if (_moveCorounine != null)
            _character.StopCoroutine(_moveCorounine);

        IsMoveToTarget = true;
        _moveCorounine = _character.StartCoroutine(MoveJob(target, offset));
        _agent.isStopped = false;
    }

    public void StopMove()
    {
        if(_moveCorounine != null)
            _character.StopCoroutine(_moveCorounine);

        _agent.ResetPath();
        _moveCorounine = null;
        IsMoveToTarget = false;
    }

    private IEnumerator MoveJob(Transform target, float offset)
    {
        //WaitForSeconds time = new WaitForSeconds(_timeForDistanceReconculate);

        while (Vector3.Distance(target.position, _character.transform.position) > offset)
        {
            _agent.SetDestination(target.position);
            yield return null;
        }
        IsMoveToTarget = false;
        ReachedEndPoint?.Invoke();
        _agent.ResetPath();
    }
}
