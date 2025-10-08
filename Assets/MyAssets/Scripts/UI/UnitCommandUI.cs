using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Zenject;

public class UnitCommandUI : MonoBehaviour
{
    [SerializeField] private Button _attackUniysMode;
    [SerializeField] private Button _attackBaseMode;
    [SerializeField] private Button _defenseMode;

    private Selector _selector;
    private UnitCommands _command;
    private Character _character;

    [Inject]
    public void Inject(Selector selector)
    {
        _selector = selector;
    }

    private void Awake()
    {
        //gameObject.SetActive(false);

        _selector.Selected += OnSelected;
        _selector.Deselected += OnDeselected;

        _attackUniysMode.onClick.AddListener(OnAttackUniysMode);
        _attackBaseMode.onClick.AddListener(OnAttackBaseMode);
        _defenseMode.onClick.AddListener(OnDefenseMode);
    }

    private void OnDestroy()
    {
        _selector.Selected -= OnSelected;
        _selector.Deselected -= OnDeselected;
    }

    private void OnSelected(ISelectable selectable)
    {
        if (selectable is Character character)
        {
            //gameObject.SetActive(true);
            _character = character;
            _character.SetMode(_command);
        }
        else
        {
            //gameObject.SetActive(false);
            _character = null;
        }
    }
    
    private void OnDeselected(ISelectable selectable)
    {
        //gameObject.SetActive(false);
        _character = null;
    }

    private void OnDefenseMode()
    {
        _command = UnitCommands.Defense;
        //_character.SetMode(_command);
    }

    private void OnAttackBaseMode()
    {
        _command = UnitCommands.PushBase;
        //_character.SetMode(_command);
    }

    private void OnAttackUniysMode()
    {
        _command = UnitCommands.MoveAndAttak;
        //_character.SetMode(_command);
    }
}
