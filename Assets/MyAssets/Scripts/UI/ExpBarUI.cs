using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private Character _character;

    public void Init(Character character)
    {
        _character = character;

        OnValueChanged(0, 1);

        _character.ExpChanged += OnValueChanged;
    }

    private void OnDestroy()
    {
        _character.ExpChanged -= OnValueChanged;
    }

    private void OnValueChanged(float value, float MaxValue)
    {
        _slider.value = value / MaxValue;
    }
}
