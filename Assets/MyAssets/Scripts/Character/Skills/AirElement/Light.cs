using System.Collections;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

namespace AirElement
{
    public class Light : Skill
    {
        [SerializeField] private float _damage = 20;
        [SerializeField] private float _radius = 5;
        [SerializeField] private float _damageReduction = 0.7f;

        protected override IEnumerator CastJob(ITargetable target)
        {
            DealDamage(target, 1);

            var colliders = Physics.OverlapSphere(Character.transform.position, _radius, Layers.EnemyMask);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ITargetable subTarget) && subTarget.Transform != target.Transform)
                {
                    DealDamage(target, _damageReduction);
                    yield break;
                }
            }
            yield return null;
        }

        protected override void ClearData()
        {

        }

        private void DealDamage(ITargetable targetable, float damageMultiple)
        {
            if (targetable is IDamageable damageable)
                Character.DealDamage(_damage, targetable.Transform.gameObject);
        }
    }
}
