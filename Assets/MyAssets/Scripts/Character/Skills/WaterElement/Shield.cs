using System.Collections;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace WaterElement
{
    public class Shield : Skill
    {
        [SerializeField] private float _hp = 40f;
        [SerializeField] private float _duration = 5;

        private Coroutine _shieldCoroutine;
        private float _currentShieldHP;

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
            _currentShieldHP = _hp;

            Character.BeforDamageTaked += OnBeforDamageTaked;
            yield return new WaitForSeconds(_duration);
            Character.BeforDamageTaked -= OnBeforDamageTaked;
        }

        private void OnBeforDamageTaked(Damage damage)
        {
            _currentShieldHP -= damage.Value;

            if (_currentShieldHP < 0)
            {
                damage.Damageable.TempDamageValue = math.abs(_currentShieldHP);
                StopCoroutine(_shieldCoroutine);
                Character.BeforDamageTaked -= OnBeforDamageTaked;
            }
            else
            {
                damage.Damageable.TempDamageValue = 0;
            }
        }

        [Command]
        private void CmdStartCoroutineDamageReduction()
        {
            _shieldCoroutine = StartCoroutine(DamageReductionJob());
        }
    }
}