using System;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private float _maxValue;

    public void Init(Character character)
    {
        _maxValue = character.Health;

        OnValueChanged(_maxValue, _maxValue);

        character.HPChanged += OnValueChanged;
    }

    public void Init(Base character)
    {
        _maxValue = character.Health;

        OnValueChanged(_maxValue, _maxValue);

        character.HPChanged += OnValueChanged;
    }

    private void OnDestroy()
    {
        
    }

    private void OnValueChanged(float value, float newValue)
    {
        _slider.value = newValue / _maxValue;
    }
}
