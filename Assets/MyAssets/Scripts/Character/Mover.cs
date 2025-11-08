using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mover
{
    private NavMeshAgent _agent;
    private Character _character;
    private Coroutine _moveCorounine;
    private int _disableCounter = 0;

    public bool IsMove { get; private set; }

    public event Action ReachedEndPoint;

    public Mover(Character character)
    {
        _character = character;
        _agent = _character.Agent;
        _agent.speed = character.MoveSpeed;
    }

    public void Disable()
    {
        _disableCounter++;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
    }

    public void Enable()
    {
        if (_disableCounter > 0)
            _disableCounter--;

        if (_disableCounter == 0)
            _agent.isStopped = false;
    }

    public void MoveTo(Transform target, float offset)
    {
        if (_moveCorounine != null)
            _character.StopCoroutine(_moveCorounine);

        IsMove = true;
        _moveCorounine = _character.StartCoroutine(MoveJob(target, offset));
    }

    public void MoveTo(Vector3 point, float offset)
    {
        if (_moveCorounine != null)
            _character.StopCoroutine(_moveCorounine);

        IsMove = true;
        _moveCorounine = _character.StartCoroutine(MoveJob(point, offset));
    }

    public void StopMove()
    {
        if(_moveCorounine != null)
            _character.StopCoroutine(_moveCorounine);

        _agent.ResetPath();
        _moveCorounine = null;
        IsMove = false;
    }

    private IEnumerator MoveJob(Transform target, float offset)
    {
        while (Vector3.Distance(target.position, _character.transform.position) > offset)
        {
            _agent.SetDestination(target.position);
            yield return null;
        }
        IsMove = false;
        _agent.ResetPath();
        ReachedEndPoint?.Invoke();
    }

    private IEnumerator MoveJob(Vector3 target, float offset)
    {
        _agent.SetDestination(target);

        while (Vector3.Distance(target, _character.transform.position) > offset)
        {
            yield return null;
        }
        IsMove = false;
        _agent.ResetPath();
        ReachedEndPoint?.Invoke();
    }
}
