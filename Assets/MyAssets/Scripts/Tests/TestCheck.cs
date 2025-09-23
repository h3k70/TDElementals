using UnityEngine;

public class TestCheck : MonoBehaviour
{
    private Collider[] _results;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        Physics.OverlapSphereNonAlloc(transform.position, 40, _results);
    }
}
