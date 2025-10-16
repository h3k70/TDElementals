using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _barImage;
    [SerializeField] private Color _allyColor;
    [SerializeField] private Color _enemyColor;

    private float _maxValue;

    public void Init(Character character)
    {
        _maxValue = character.Health;

        OnValueChanged(_maxValue, _maxValue);

        character.HPChanged += OnValueChanged;
        character.MaxHPChanged += OnMaxValueChanged;

        StartCoroutine(ChangeColorJob(character.gameObject));
    }

    public void Init(Base character)
    {
        _maxValue = character.Health;

        OnValueChanged(_maxValue, _maxValue);

        character.HPChanged += OnValueChanged;

        StartCoroutine(ChangeColorJob(character.gameObject));
    }

    private void OnDestroy()
    {
        
    }

    private void OnValueChanged(float value, float newValue)
    {
        _slider.value = newValue / _maxValue;
    }

    private void OnMaxValueChanged(float value, float newValue)
    {
        _maxValue = newValue;
    }

    private IEnumerator ChangeColorJob(GameObject character)
    {
        float time = 1f;

        while (time > 0)
        {
            if (character.gameObject.layer == Layers.Ally)
            {
                _barImage.color = _allyColor;
            }
            else
            {
                _barImage.color = _enemyColor;
            }
            yield return null;
            time -= Time.deltaTime;
        }
    }
}
