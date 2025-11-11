using System.Collections;
using Mirror;
using UnityEngine;

namespace AirElement
{
    public class Light : Skill
    {
        [SerializeField] private float _damage = 20;

        protected override IEnumerator CastJob(ITargetable target)
        {
            if (target is IDamageable damageable)
                Character.DealDamage(_damage, target.Transform.gameObject);
            yield return null;
        }

        protected override void ClearData()
        {

        }
    }
}
