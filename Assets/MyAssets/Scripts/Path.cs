using System;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour, ISelectable
{
    [SerializeField] private List<Transform> _points = new();

    public List<Transform> Points { get => _points; }

    public Sprite Icon => throw new NotImplementedException();

    public bool IsSelected { get; private set; }

    public GameObject Self => gameObject;

    private event Action<ISelectable> _selected;
    public event Action<bool> Selected;
    public event Action<ISelectable> Deselected;
    event Action<ISelectable> ISelectable.Selected
    {
        add
        {
            _selected += value;
        }

        remove
        {
            _selected -= value;
        }
    }

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

    public void SetSelect(bool value)
    {
        Selected?.Invoke(value);
    }

    [ContextMenu(nameof(Flip))]
    public void Flip()
    {
        _points.Reverse();
    }

    public void Select()
    {
        
    }

    public void Deselect()
    {
        
    }
}
