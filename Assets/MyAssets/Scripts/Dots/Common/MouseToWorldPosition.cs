using UnityEngine;

public class MouseToWorldPosition : MonoBehaviour
{
    public static MouseToWorldPosition Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;

    }

    public Vector3 GetPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(mouseCameraRay, out RaycastHit raycastHit))
        {
              return raycastHit.point;
        }
        return Vector3.zero;
    }
}
