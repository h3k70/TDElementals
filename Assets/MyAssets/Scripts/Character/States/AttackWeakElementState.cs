using System.Collections.Generic;
using UnityEngine.AI;

public class AttackWeakElementState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private AgentMover _mover;
    private List<Character> _enemies;
    private float _offset = 1.5f;
    private Elements _weekElement;

    public AttackWeakElementState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _enemies = character.EnemyChecker.Enemies;
        _mover = new AgentMover(character.GetComponent<NavMeshAgent>(), character, character.Path);
        _stateMachine = stateMachine;
        ElementsCross.StrongWeak.TryGetValue(_character.Element, out _weekElement);
    }

    public void Enter()
    {
        _path = _character.Path;
        _mover.SetPath(_path);
        _mover.ReachedEndPoint += OnReachedEndPoint;
    }

    public void Exit()
    {
        _mover.ReachedEndPoint -= OnReachedEndPoint;
    }

    public void Update()
    {
        foreach (Character enemy in _enemies)
        {
            if (enemy.Element != _weekElement)
                continue;

            _mover.Pause();

            if (enemy.IsCanTakeDamage)
            {
                _character.TryAttack(enemy);
                return;
            }
        }
        _mover.Resume();
        _mover.Update();
    }

    private void OnReachedEndPoint()
    {
        _stateMachine.SwitchState<AttackBaseBuildState>();
    }
}
