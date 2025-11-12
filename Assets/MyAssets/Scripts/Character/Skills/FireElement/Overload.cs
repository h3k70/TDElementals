using System.Collections;
using UnityEngine;

namespace FireElement
{
    public class Overload : Skill
    {
        [SerializeField] private float _speedReduction = 2f;
        [SerializeField] private float _duration = 4;

        protected override IEnumerator CastJob(ITargetable target)
        {
            StartCoroutine(SpeedReductionJob());
            yield return null;
        }

        protected override void ClearData()
        {

        }

        private IEnumerator SpeedReductionJob()
        {
            Character.AttackRate = Character.AttackRate * _speedReduction;
            yield return new WaitForSeconds(_duration);
            Character.AttackRate = Character.AttackRate / _speedReduction;
        }
    }
}