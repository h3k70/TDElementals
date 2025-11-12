using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace AirElement
{
    public class ClearLight : Skill
    {
        [SerializeField] private float _speedBuff = 1.5f;
        [SerializeField] private float _duration = 5;
        [SerializeField] private float _radius = 6;

        private Character _target;

        public void AnimEventStoneThrow()
        {

        }

        protected override IEnumerator CastJob(ITargetable target)
        {
            var colliders = Physics.OverlapSphere(Character.transform.position, _radius, Layers.AllyMask);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Character character))
                {
                    CmdAddState(character.gameObject, Attributes.MoveSpeed);
                    StartCoroutine(RemoveBuffJob(character));
                }
            }
            yield return null;
        }

        protected override void ClearData()
        {
        }

        private IEnumerator RemoveBuffJob(Character character)
        {
            yield return new WaitForSeconds(_duration);
            CmdRemoveState(character.gameObject, Attributes.MoveSpeed);
        }

        [Command]
        private void CmdAddState(GameObject target, Attributes state)
        {
            target.GetComponent<Character>().BuffAttribute(Attributes.MoveSpeed, _speedBuff);
        }

        [Command]
        private void CmdRemoveState(GameObject target, Attributes state)
        {
            target.GetComponent<Character>().RemoveBuffAttribute(Attributes.MoveSpeed, _speedBuff);
        }
    }
}