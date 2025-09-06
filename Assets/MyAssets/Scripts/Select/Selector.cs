using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector
{
    private List<ISelectable> _allSelectablsUnit = new();
    private List<ISelectable> _currentSelectablsUnit = new();
    private List<ISelectable> _tempForDeleteSelectablsUnit = new();
    private Camera _camera;
    private GameInputMap _input;
    private float _rayDistance = 9999;
    private int _layerMask = 384; //Ally and Enemy

    public Selector(GameInputMap inputActions)
    {
        _input = inputActions;
        _input.Enable();
        _camera = Camera.main;

        _input.Gameplay.SelectUnit.performed += OnSelectUnit;
    }

    private void OnSelectUnit(InputAction.CallbackContext context)
    {
        Ray ray = _camera.ScreenPointToRay(context.ReadValue<Vector2>());

        foreach (var item in _currentSelectablsUnit)
        {
            item.Deselect();
            _tempForDeleteSelectablsUnit.Add(item);
        }
        foreach (var item in _tempForDeleteSelectablsUnit)
        {
            _currentSelectablsUnit.Remove(item);
        }

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _rayDistance, _layerMask) && hitInfo.collider.TryGetComponent(out ISelectable unit))
        {
            unit.Select();
            _currentSelectablsUnit.Add(unit);
        }
    }
}
