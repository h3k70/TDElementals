using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private SelectUnitPanelUI _unitPanelUI;
    [SerializeField] private Button _swichLeftPathButton;
    [SerializeField] private Button _swichRightPathButton;
    [SerializeField] private TMP_Text _buttlePoints;
    [SerializeField] private UnitCommandUI _unitCommandUI;
    [SerializeField] private List<BuffButtonUI> _buffButtons;

    public SelectUnitPanelUI UnitPanelUI => _unitPanelUI;

    public Button SwichLeftPathButton { get => _swichLeftPathButton; }
    public Button SwichRightPathButton { get => _swichRightPathButton; }
    public TMP_Text ButtlePoints { get => _buttlePoints; }
    public UnitCommandUI UnitCommandUI { get => _unitCommandUI; }
    public List<BuffButtonUI> BuffButtons { get => _buffButtons; }

    public void OnBattlePointsChanged(float oldValue, float newValue)
    {
        _buttlePoints.text = newValue.ToString();
    }

    public void ResetAll()
    {
        _unitPanelUI.ResetAll();
    }
}
