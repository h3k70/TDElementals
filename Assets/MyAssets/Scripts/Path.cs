using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new();

    public Transform GetCloserPoint(Vector3 position)
    {
        float closerDistance = float.PositiveInfinity;
        float distance = 0;
        Transform closerPoint = null;

        foreach (Transform t in _points)
        {
            distance = Vector3.Distance(position, t.position);

            if (distance < closerDistance)
            {
                closerDistance = distance;
                closerPoint = t;
            }
        }
        return closerPoint;
    }

    public Transform GetNextPoint(Transform point)
    {
        int currentIndex = _points.IndexOf(point);
        
        if (currentIndex + 1 < _points.Count)
            return _points[currentIndex + 1];
        else
            return null;
    }

    [ContextMenu(nameof(Flip))]
    public void Flip()
    {
        _points.Reverse();
    }
}
