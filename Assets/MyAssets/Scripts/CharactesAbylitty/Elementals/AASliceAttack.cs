using System.Collections;
using UnityEngine;

public class AASliceAttack : Skill
{
    private float _damage;
    private bool _isAnimPlaying;
    private IDamageable _target;

    public void AnimEventAASliceAttack()
    {
        if (_target != null)
            Character.DealDamage(_damage, _target.Self);
    }

    public void AnimEventAASliceAttackEnded()
    {
        _isAnimPlaying = false;
    }

    protected override IEnumerator CastJob(ITargetable target)
    {
        Cooldown = 1 / Character.AttackRate;
        Character.Animator.SetFloat("Attack Speed", 1 * Character.AttackRate);

        _damage = Character.Damage;
        transform.LookAt(target.Transform.position);
        Character.NetAnimator.SetTrigger("Slice Attack");
        _isAnimPlaying = true;

        if (target is IDamageable damageable)
            _target = damageable;

        while (_isAnimPlaying)
        {
            transform.LookAt(target.Transform.position);
            yield return null;
        }
    }

    protected override void ClearData()
    {
        _isAnimPlaying = false;
        _target = null;
    }
}
