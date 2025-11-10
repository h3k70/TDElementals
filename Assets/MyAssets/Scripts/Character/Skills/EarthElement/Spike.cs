using System;
using System.Collections;
using Mirror;
using UnityEngine;

namespace EarthElement
{
    public class Spike : Skill
    {
        [SerializeField] private float _damageReduction = .2f;
        [SerializeField] private float _duration = 5;

        protected override IEnumerator CastJob(ITargetable target)
        {
            CmdStartCoroutineDamageReduction();
            yield return null;
        }

        protected override void ClearData()
        {
            
        }

        private IEnumerator DamageReductionJob()
        {
            Character.BeforDamageTaked += OnBeforDamageTaked;
            yield return new WaitForSeconds(_duration);
            Character.BeforDamageTaked -= OnBeforDamageTaked;
        }

        private void OnBeforDamageTaked(Damage damage)
        {
            damage.Damageable.TempDamageValue = damage.Value * _damageReduction;
        }

        [Command]
        private void CmdStartCoroutineDamageReduction()
        {
            StartCoroutine(DamageReductionJob());
        }
    }
}
