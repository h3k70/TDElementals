using UnityEngine;

public class AttackBaseBuildState : IState
{
    private Character _character;
    private float _searchRadius = 10;
    private IDamageable _target;

    public AttackBaseBuildState(Character character)
    {
        _character = character;
    }

    public void Enter()
    {
        var colliders = Physics.OverlapSphere(_character.transform.position, _searchRadius, Layers.EnemyMask);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Base build))
            {
                _target = build;
                return;
            }
        }
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (_target.IsCanTakeDamage == false)
            return;

        _character.TryAttack(_target);
    }
}