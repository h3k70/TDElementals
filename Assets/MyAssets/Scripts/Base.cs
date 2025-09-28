using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class Base : NetworkBehaviour, IDamageable, ISelectable
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private Elements _element;
    [SerializeField] private List<Character> _charactersPrefabs = new List<Character>();
    [SerializeField] private Collider _selectCollider;

    [SyncVar] private float _health = 50;
    [SyncVar] private bool _isDead;
    [SyncVar] private float _battlePoints = 0;

    private Dictionary<Character, int> _CharactersLVL = new Dictionary<Character, int>();
    private float _battlePointsTakeRate = 1;
    private float _battlePointsTakeNum = 1;
    private float _spawnUnitDeley = 10;
    private Character _selectedForSpawnUnit;
    private List<Character> _characters = new List<Character>();
    private Path _leftPath;
    private Path _rightpath;
    private Path _currentPath;
    private float _spawnOffset = 1;
    private Coroutine _regenBattlePointsJob;
    private Coroutine _autoSpawnUnitJob;

    public Sprite Icon => _icon;
    public GameObject Self => gameObject;
    public float TempDamageValue { get; set; }
    public bool IsCanTakeDamage => !_isDead;
    public bool IsSelected { get; private set; }
    public Elements Element => _element;
    public Character SelectedForSpawnUnit { get => _selectedForSpawnUnit; set => _selectedForSpawnUnit = value; }
    public float Health { get { return _health; } private set { HPChanged?.Invoke(Health, value); RpcHPChanged(Health, value); _health = value; } }
    public List<Character> CharactersPrefabs { get => _charactersPrefabs; }
    public Path CurrentPath { get => _currentPath; }
    public List<Character> Units { get => _characters; }

    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action Destroed;
    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;
    public event Action<float, float> HPChanged;
    public event Action<float, float> BattlePointsChanged;

    public void Init(List<Path> paths)
    {
        _leftPath = paths[0];
        _rightpath = paths[1];

        if (isOwned)
        {
            SelectLeftPath();
            SelectedForSpawnUnit = _charactersPrefabs[0];

            _regenBattlePointsJob = StartCoroutine(RegenBattlePointsJob());
            _autoSpawnUnitJob = StartCoroutine(AutoSpawnUnitJob());
        }
    }

    public void SelectLeftPath()
    {
        if (_currentPath != null)
            _currentPath.Select(false);

        _currentPath = _leftPath;
        _currentPath.Select(true);
    }

    public void SelecRightPath()
    {
        if (_currentPath != null)
            _currentPath.Select(false);

        _currentPath = _rightpath;
        _currentPath.Select(true);
    }

    public void Deselect()
    {
        Deselected?.Invoke(this);
    }

    public void Select()
    {
        Selected?.Invoke(this);
    }

    public void TakeDamage(Damage damage)
    {
        TempDamageValue = damage.Value;
        BeforDamageTaked?.Invoke(this);

        if (TempDamageValue > 0)
        {
            Health -= TempDamageValue;

            if (_health < 0)
            {
                _isDead = true;
                Health = 0;
                Destroed?.Invoke();
            }
            DamageTaked?.Invoke(this, damage.Value);
            RpcDamageTaked(damage.Value);
        }
    }

    public bool TrySpawnUnit(Character character)
    {
        if (character.Cost <= _battlePoints)
        {
            int index = _charactersPrefabs.FindIndex(item => item == character);
            CmdTrySpawnUnit(index, character.Cost);
            return true;
        }
        return false;
    }

    private void SpawnUnit(int index)
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-_spawnOffset, _spawnOffset), 0, Random.Range(-_spawnOffset, _spawnOffset)) + transform.position;

        Character character = Instantiate(_charactersPrefabs[index], spawnPoint, transform.rotation).GetComponent<Character>();
        _characters.Add(character);
        NetworkServer.Spawn(character.gameObject, gameObject);

        Game.Instance.RpcMarkLayerObj(character);
    }

    private IEnumerator RegenBattlePointsJob()
    {
        var time = new WaitForSeconds(_battlePointsTakeRate);

        while (true)
        {
            yield return time;
            CmdAddBattlePoints();
        }
    }

    private IEnumerator AutoSpawnUnitJob()
    {
        var time = new WaitForSeconds(_spawnUnitDeley);

        while (true)
        {
            yield return time;
            int index = _charactersPrefabs.FindIndex(item => item == _selectedForSpawnUnit);
            CmdSpawnUnit(index);
        }
    }

    [Command]
    private void CmdAddBattlePoints()
    {
        float newValue = _battlePoints + _battlePointsTakeNum;

        BattlePointsChanged?.Invoke(_battlePoints, newValue);
        RpcBattlePointsChanged(_battlePoints, newValue);
        _battlePoints = newValue;
    }

    [Command]
    private void CmdTrySpawnUnit(int index, float cost)
    {
        if (cost <= _battlePoints)
        {
            _battlePoints -= cost;

            SpawnUnit(index);
        }
    }

    [Command]
    private void CmdSpawnUnit(int index)
    {
        SpawnUnit(index);
    }

    [ClientRpc]
    private void RpcDamageTaked(float damage)
    {
        DamageTaked?.Invoke(this, damage);
    }

    [ClientRpc]
    private void RpcHPChanged(float oldValue, float newValue)
    {
        HPChanged?.Invoke(oldValue, newValue);
    }

    [ClientRpc]
    private void RpcBattlePointsChanged(float oldValue, float newValue)
    {
        BattlePointsChanged?.Invoke(oldValue, newValue);
    }
}
