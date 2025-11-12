using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : NetworkBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private List<Skill> _allSkills;
    [SerializeField] private List<Skill> _currentSkills;
    [SerializeField] private Skill _autoAtack;

    private Selector _selector;
    private GameInputMap _gameInputMap;
    private Skill _currentSkill;

    public List<Skill> AllSkills => _allSkills;
    public List<Skill> Skills => _currentSkills;
    public Skill AutoAtack { get => _autoAtack; }

    public event Action<Skill> SkillAdded;
    public event Action<Skill> SkillRemoved;
    public event Action<Skill> SkillSelected;

    public void Init()
    {
        _selector = Game.Instance.Selector;
        _gameInputMap = Game.Instance.Input;

        _character.Selected += OnSelected;
        _character.Deselected += OnDeselected;

        _currentSkill = AutoAtack;
        _autoAtack.Init(_character);
        _autoAtack.Cooldown = 1 / _character.AttackRate;
        _character.Animator.SetFloat("Attack Speed", 1 * _character.AttackRate);

        foreach (var skill in _allSkills)
        {
            skill.Init(_character);
        }

        _character.CharacterDied += OnCharacterDied;
    }

    private void OnDestroy()
    {
        _character.Selected -= OnSelected;
        _character.Deselected -= OnDeselected;
    }

    private void OnSelected(ISelectable s)
    {
        if (isOwned == false)
            return;

        _gameInputMap.Gameplay.Skill1.performed += OnSelectSkill1AA;
        _gameInputMap.Gameplay.Skill2.performed += OnSelectSkill2;
        _gameInputMap.Gameplay.Skill3.performed += OnSelectSkill3;

        _selector.SubSelected += OnSubSelected;
    }

    private void OnDeselected(ISelectable s)
    {
        if (isOwned == false)
            return;

        _gameInputMap.Gameplay.Skill1.performed -= OnSelectSkill1AA;
        _gameInputMap.Gameplay.Skill2.performed -= OnSelectSkill2;
        _gameInputMap.Gameplay.Skill3.performed -= OnSelectSkill3;

        _selector.SubSelected -= OnSubSelected;
    }

    private void OnCharacterDied(Character character)
    {
        _autoAtack.TryCancel();

        foreach (var item in _allSkills)
        {
            item.TryCancel();
        }
    }

    private void OnSelectSkill1AA(InputAction.CallbackContext context)
    {
        _currentSkill = AutoAtack;
        SkillSelected?.Invoke(_currentSkill);
    }

    private void OnSelectSkill2(InputAction.CallbackContext context)
    {
        _currentSkill = _currentSkills[0];
        SkillSelected?.Invoke(_currentSkill);
    }

    private void OnSelectSkill3(InputAction.CallbackContext context)
    {
        _currentSkill = _currentSkills[1];
        SkillSelected?.Invoke(_currentSkill);
    }

    private void OnSubSelected(ISelectable selectable)
    {
        if (selectable is ITargetable target)
        {
            _character.DisableAutoAttack();
            AutoAtack.TryCancel();
            _currentSkill.CastEnded += OnCastEnded;

            if (_currentSkill.TryCast(target) == false)
                OnCastEnded(_currentSkill);
        }
    }

    private void OnCastEnded(Skill skill)
    {
        _currentSkill.CastEnded -= OnCastEnded;
        _character.EnableAutoAttack();
    }
}
