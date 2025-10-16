using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Selector
{
    private List<ISelectable> _allSelectablsUnit = new();
    private List<ISelectable> _currentSelectablsUnit = new();
    private List<ISelectable> _tempForDeleteSelectablsUnit = new();
    private Camera _camera;
    private GameInputMap _input;
    private float _rayDistance = 9999;
    private int _layerMask = Layers.AllyMask;

    public List<ISelectable> AllSelectablsUnit { get => _allSelectablsUnit; set => _allSelectablsUnit = value; }
    public List<ISelectable> CurrentSelectablsUnit { get => _currentSelectablsUnit; }

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> SubSelected;
    public event Action<ISelectable> Deselected;

    public Selector(GameInputMap inputActions)
    {
        _input = inputActions;
        _input.Enable();
        _camera = Camera.main;

        _input.Gameplay.SelectUnit.performed += OnSelectUnit;
        _input.Gameplay.SubSelectUnit.performed += OnSubSelect;
    }

    private void OnSelectUnit(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = _camera.ScreenPointToRay(context.ReadValue<Vector2>());

        foreach (var item in _currentSelectablsUnit)
        {
            if (item != null)
            {
                item.Deselect();
                Deselected?.Invoke(item);
            }

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

            Selected?.Invoke(unit);
        }
    }

    private void OnSubSelect(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = _camera.ScreenPointToRay(context.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _rayDistance, _layerMask) && hitInfo.collider.TryGetComponent(out ISelectable unit))
        {
            SubSelected?.Invoke(unit);
        }
    }
}
