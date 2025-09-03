using System.Collections.Generic;
using UnityEngine;

public class Selector
{
    private List<ISelectable> _selectabls = new();
    private Camera _camera;
    
    public Selector()
    {
        _camera = Camera.main;
    }
}
