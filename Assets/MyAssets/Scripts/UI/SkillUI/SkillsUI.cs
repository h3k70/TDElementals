using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class SkillsUI : MonoBehaviour
{
    [SerializeField] private SkillIconUI _skillIconPref;

    private List<SkillIconUI> _skillIcons = new();
    private Selector _selector;
    private Character _character;

    [Inject]
    public void Init(Selector selector)
    {
        _selector = selector;

        _selector.Selected += OnSelected;
        _selector.Deselected += OnDeselected;
    }

    private void OnSelected(ISelectable selectable)
    {
        if (selectable is Character character)
        {
            _character = character;

            ClearSkills();

            foreach (var item in _character.Skills.Skills)
            {
                var icon = Instantiate(_skillIconPref, transform);
                _skillIcons.Add(icon);
                icon.Init(item);
            }
        }
    }

    private void OnDeselected(ISelectable selectable)
    {
        ClearSkills();
    }

    private void ClearSkills()
    {
        foreach (var item in _skillIcons)
            Destroy(item.gameObject);

        _skillIcons.Clear();
    }
}
