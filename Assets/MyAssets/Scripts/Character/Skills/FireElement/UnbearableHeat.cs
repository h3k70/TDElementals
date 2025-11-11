using System.Collections;
using Mirror;
using UnityEngine;

namespace FireElement
{
    public class UnbearableHeat : Skill
    {
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _duration = 5;
        [SerializeField] private float _radius = 5;
        [SerializeField] private float _deley = 1;

        protected override IEnumerator CastJob(ITargetable target)
        {
            StartCoroutine(DamageReductionJob());
            yield return null;
        }

        protected override void ClearData()
        {

        }

        private IEnumerator DamageReductionJob()
        {
            var timeDeley = new WaitForSeconds(_deley);
            float time = _duration;

            while (time > 0)
            {
                yield return timeDeley;

                var colliders = Physics.OverlapSphere(Character.transform.position, _radius, Layers.EnemyMask);

                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out IDamageable damageable))
                    {
                        Character.DealDamage(_damage, damageable.Self);
                    }
                }
                time -= _deley;
            }
        }
    }
}
