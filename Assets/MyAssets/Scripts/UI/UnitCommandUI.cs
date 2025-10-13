using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UnitCommandUI : MonoBehaviour
{
    [SerializeField] private Button _attackUniysMode;
    [SerializeField] private Button _attackBaseMode;
    [SerializeField] private Button _defenseMode;
    [SerializeField] private Button _attackWeakMode;

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
        _attackWeakMode.onClick.AddListener(OnAttackWeakMode);
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

        if (_character != null)
            _character.SetMode(_command);
    }

    private void OnAttackBaseMode()
    {
        _command = UnitCommands.PushBase;

        if (_character != null)
            _character.SetMode(_command);
    }

    private void OnAttackUniysMode()
    {
        _command = UnitCommands.MoveAndAttak;

        if (_character != null)
            _character.SetMode(_command);
    }

    private void OnAttackWeakMode()
    {
        _command = UnitCommands.MoveAndAttackWeak;

        if (_character != null)
            _character.SetMode(_command);
    }
}
