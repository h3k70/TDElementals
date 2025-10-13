using UnityEngine;

public class Testover : MonoBehaviour
{
    private float _searchRadius = 10;

    [ContextMenu("asd")]
    public void asd()
    {
        var colliders = Physics.OverlapSphere(transform.position, _searchRadius, LayerMask.NameToLayer("Enemy"));
        Debug.Log(colliders.Length);

        foreach (var collider in colliders)
        {
            Debug.Log(collider.gameObject.name);
        }
    }
}
