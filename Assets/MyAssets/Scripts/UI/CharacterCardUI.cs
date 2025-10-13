using System;
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
    [SerializeField] private TMP_Text _lvl;
    [SerializeField] private ExpBarUI _expBar;
    [SerializeField] private Button _button;
    [SerializeField] private Image _cooldown;

    private Character _character;
    private Character _characterPref;
    private Base _base;

    public Character Character { get => _character; }
    public Character CharacterPref { get => _characterPref; }

    public event Action<CharacterCardUI> CharacterCardSelected;

    public void Init(Character character, Character characterPref)
    {
        _button.onClick.AddListener(OnClick);

        _base = Game.Instance.OwnerBase;
        _base.SpawnUnitDeleyPointChanged += OnSpawnUnitDeleyPointChanged;

        _character = character;
        _characterPref = characterPref;
        _character.LVLChanged += OnLVLChanged;
        _character.CostChanged += OnCostChanged;

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

        _expBar.Init(character);
    }

    private void OnCostChanged(float obj)
    {
        _cost.text = obj.ToString();
    }

    private void OnLVLChanged(int arg1, int arg2)
    {
        _lvl.text = arg2.ToString();
    }

    public void OnDestroy()
    {
        _base.SpawnUnitDeleyPointChanged -= OnSpawnUnitDeleyPointChanged;
        _character.LVLChanged -= OnLVLChanged;
        _character.CostChanged -= OnCostChanged;
    }

    public void SetSelect(bool value)
    {
        _selectFrame.gameObject.SetActive(value);
    }

    private void OnClick()
    {
        CharacterCardSelected?.Invoke(this);
    }

    private void OnSpawnUnitDeleyPointChanged(float obj)
    {
        _cooldown.fillAmount = obj / _character.Cost;
    }
}
