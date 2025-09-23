using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _selectFrame;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private Sprite[] _elementIcons;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private TMP_Text _damage;
    [SerializeField] private TMP_Text _damageRate;
    [SerializeField] private TMP_Text _moveSpeed;
    [SerializeField] private TMP_Text _hp;
    [SerializeField] private Button _button;

    private Character _character;

    public Character Character { get => _character; }

    public event Action<CharacterCardUI> CharacterCardSelected;

    public void Init(Character character)
    {
        _button.onClick.AddListener(OnClick);

        _character = character;

        _icon.sprite = _character.Icon;

        switch (character.Element)
        {
            case Elements.None:
                break;

            case Elements.Air:
                _elementIcon.sprite = _elementIcons[0];
                break;

            case Elements.Fire:
                _elementIcon.sprite = _elementIcons[1];
                break;

            case Elements.Water:
                _elementIcon.sprite = _elementIcons[2];
                break;

            case Elements.Earth:
                _elementIcon.sprite = _elementIcons[3];
                break;

            default:
                break;
        }

        _cost.text = _character.Cost.ToString();
        _damage.text = _character.Damage.ToString();
        _damageRate.text = _character.AttackRate.ToString();
        _moveSpeed.text = _character.MoveSpeed.ToString();
        _hp.text = _character.Health.ToString();
    }

    public void SetSelect(bool value)
    {
        _selectFrame.gameObject.SetActive(value);
    }

    private void OnClick()
    {
        CharacterCardSelected?.Invoke(this);
    }
}
