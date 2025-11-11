using System.Collections;
using Mirror;
using UnityEngine;

namespace WaterElement
{
    public class Freeze : Skill
    {
        [SerializeField] private float _radius = 5;
        [SerializeField] private float _stanDuration = 6;

        protected override IEnumerator CastJob(ITargetable target)
        {
            var colliders = Physics.OverlapSphere(Character.transform.position, _radius, Layers.EnemyMask);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Character character))
                {
                    CmdAddState(character.gameObject, States.Freeze);
                }
            }
            yield return null;
        }

        protected override void ClearData()
        {

        }


        [Command]
        private void CmdAddState(GameObject target, States state)
        {
            target.GetComponent<Character>().AddState(state, _stanDuration);
        }
    }
}
