using System;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private SelectUnitPanelUI _unitPanelUI;
    [SerializeField] private Button _swichLeftPathButton;
    [SerializeField] private Button _swichRightPathButton;

    public SelectUnitPanelUI UnitPanelUI => _unitPanelUI;

    public Button SwichLeftPathButton { get => _swichLeftPathButton; }
    public Button SwichRightPathButton { get => _swichRightPathButton; }

    public void ResetAll()
    {
        _unitPanelUI.ResetAll();
    }
}
