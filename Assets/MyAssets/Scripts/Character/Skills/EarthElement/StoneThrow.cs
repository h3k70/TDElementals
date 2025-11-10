using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace EarthElement
{
    public class StoneThrow : Skill
    {
        [SerializeField] private float _damage = 20;
        [SerializeField] private float _stanDuration = 4;

        private bool _isAnimPlaying;
        private IDamageable _target;

        public void AnimEventStoneThrow()
        {
            if (_target != null)
            {
                Character.DealDamage(_damage, _target.Self);

                if (_target is Character character)
                {
                    CmdAddState(character.gameObject, States.Stun);
                }
            }
        }

        public void AnimEventStoneThrowEnded()
        {
            _isAnimPlaying = false;
        }

        protected override IEnumerator CastJob(ITargetable target)
        {
            transform.LookAt(target.Transform.position);
            Character.NetAnimator.SetTrigger("StoneThrow");
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

        [Command]
        private void CmdAddState(GameObject target, States state)
        {
            target.GetComponent<Character>().AddState(States.Stun, _stanDuration);
        }
    }
}