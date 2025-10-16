using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Random = UnityEngine.Random;

public class Base : NetworkBehaviour, IDamageable, ISelectable
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private Elements _element;
    [SerializeField] private List<Character> _charactersPrefabs = new List<Character>();
    [SerializeField] private Collider _selectCollider;

    [SyncVar] private float _health = 600;
    [SyncVar] private bool _isDead;
    [SyncVar] private float _battlePoints = 0;
    [SyncVar] private float _maxUnits = 1;

    [SerializeField] private List<Character> _charactersForCards = new List<Character>();
    private float _battlePointsTakeRate = 30;
    private float _battlePointsTakeNum = 10;
    private float _spawnUnitDeleyPoint = 0;
    private Character _selectedForSpawnUnit;
    private Character _selectedCardUnit;
    private List<Character> _characters = new List<Character>();
    private Path _leftPath;
    private Path _rightPath;
    private Path _currentPath;
    private float _spawnOffset = 1;
    private float _maxUnitsDeley = 180;
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
    public List<Character> CharactersForCards { get => _charactersForCards; }
    public Path CurrentPath { get => _currentPath; }
    public List<Character> Units { get => _characters; }
    public int MaxUnitLVL { get => 10; }
    public float BattlePoints { get => _battlePoints; }
    public float SpawnUnitDeleyPoint { get => _spawnUnitDeleyPoint; }
    public Path LeftPath { get => _leftPath; }
    public Path RightPath { get => _rightPath; }
    public Character SelectedCardUnit { get { return _selectedCardUnit; } set { _selectedCardUnit = value; } }

    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action Destroed;
    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;
    public event Action<float, float> HPChanged;
    public event Action<float, float> BattlePointsChanged;
    public event Action<Character> CharacterSpawned;
    public event Action<float> SpawnUnitDeleyPointChanged;

    public void Init(List<Path> paths)
    {
        _leftPath = paths[0];
        _rightPath = paths[1];

        if (isOwned)
        {
            _leftPath.gameObject.layer = Layers.Ally;
            _rightPath.gameObject.layer = Layers.Ally;

            SelectLeftPath();
            SelectedForSpawnUnit = _charactersPrefabs[0];
            _selectedCardUnit = _charactersForCards[0];

            _regenBattlePointsJob = StartCoroutine(RegenBattlePointsJob());
            _autoSpawnUnitJob = StartCoroutine(AutoSpawnUnitJob());

            BattlePointsChanged?.Invoke(_battlePoints, _battlePoints);
        }
        if (isClient == false)
        {
            StartCoroutine(AddMaxUnitsJob());
        }
    }

    public void SelectLeftPath()
    {
        if (_currentPath != null)
            _currentPath.SetSelect(false);

        _currentPath = _leftPath;
        _currentPath.SetSelect(true);
    }

    public void SelecRightPath()
    {
        if (_currentPath != null)
            _currentPath.SetSelect(false);

        _currentPath = _rightPath;
        _currentPath.SetSelect(true);
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

    public bool TrySpawnUnit(Character character, float cost)
    {
        if (cost <= _battlePoints && _characters.Count < _maxUnits)
        {
            int index = _charactersPrefabs.FindIndex(item => item == character);
            CmdTrySpawnUnit(index, cost);
            return true;
        }
        return false;
    }

    public void BuffUnitInRadius(Buffs buff, float value, float time)
    {
        List<Character> list = new List<Character>();

        foreach (Character character in _characters)
        {
            character.Buff(buff, value);
            list.Add(character);
        }
        StartCoroutine(DebufUnitsJob(buff, value, list, time));
    }

    private void SpawnUnit(int index)
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-_spawnOffset, _spawnOffset), 0, Random.Range(-_spawnOffset, _spawnOffset)) + transform.position;

        Character character = Instantiate(_charactersPrefabs[index], spawnPoint, transform.rotation).GetComponent<Character>();
        character.SelfCard = _charactersForCards[index];
        _characters.Add(character);

        character.SetLVL(_charactersForCards[index].CurrentLVL);

        NetworkServer.Spawn(character.gameObject, gameObject);

        Game.Instance.RpcMarkLayerObj(character);

        CharacterSpawned?.Invoke(character);
        RpcCharacterSpawned(character.gameObject);
    }

    private void OnCharacterDied(Character character)
    {
        character.CharacterDied -= OnCharacterDied;

        _characters.Remove(character);
    }

    private IEnumerator RegenBattlePointsJob()
    {
        var time = new WaitForSeconds(_battlePointsTakeRate);
        int num = 0;

        while (true)
        {
            yield return time;
            num++;
            CmdAddBattlePoints(_battlePointsTakeNum);
        }
    }

    private IEnumerator AutoSpawnUnitJob()
    {
        _spawnUnitDeleyPoint = 0;

        while (true)
        {
            yield return null;

            _spawnUnitDeleyPoint += Time.deltaTime;
            SpawnUnitDeleyPointChanged?.Invoke(SpawnUnitDeleyPoint);

            int index = _charactersPrefabs.FindIndex(item => item == _selectedForSpawnUnit);

            if (_charactersForCards.Count > 0 && _charactersForCards[index].Cost <= _spawnUnitDeleyPoint && _characters.Count < _maxUnits)
            {
                CmdSpawnUnit(index);
                _spawnUnitDeleyPoint = 0;
                SpawnUnitDeleyPointChanged?.Invoke(SpawnUnitDeleyPoint);
            }
        }
    }

    private IEnumerator AddMaxUnitsJob()
    {
        while (true)
        {
            yield return new WaitForSeconds(_maxUnitsDeley);
            _maxUnits++;
        }
    }

    private IEnumerator BuffUnitInRadiusJob(Buffs buff, float value, float time)
    {
        List<Character> list = new List<Character>();
        float radius = 10;
        float deley = 1;
        var deleyTime = new WaitForSeconds(deley);

        while (time > 0) 
        {
            foreach (Character character in _characters)
            {
                if (Vector3.Distance(character.transform.position, transform.position) > radius)
                {
                    if (list.Contains(character))
                    {
                        list.Remove(character);
                        character.Debuff(buff, value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (list.Contains(character) == false)
                {
                    character.Buff(buff, value);
                    list.Add(character);
                }

            }

            yield return deleyTime;
            time -= deley;
        }

        foreach (Character character in list)
        {
            character.Debuff(buff, value);
        }
    }

    private IEnumerator DebufUnitsJob(Buffs buff, float value, List<Character> list, float time)
    {
        yield return new WaitForSeconds(time);

        foreach (Character character in list)
        {
            if (character != null)
                character.Debuff(buff, value);
        }
    }

    [Command]
    public void CmdBuffUnitInRadius(Buffs buff, float value, float time)
    {
        //StartCoroutine(BuffUnitInRadiusJob(buff, value, time));
        BuffUnitInRadius(buff, value, time);
    }

    [Command]
    public void CmdPayCost(float value)
    {
        float newValue = _battlePoints - value;
        BattlePointsChanged?.Invoke(_battlePoints, newValue);
        RpcBattlePointsChanged(_battlePoints, newValue);
        _battlePoints = newValue;
    }

    [Command]
    private void CmdAddBattlePoints(float value)
    {
        float newValue = _battlePoints + value;

        BattlePointsChanged?.Invoke(_battlePoints, newValue);
        RpcBattlePointsChanged(_battlePoints, newValue);
        _battlePoints = newValue;
    }

    [Command]
    private void CmdTrySpawnUnit(int index, float cost)
    {
        if (cost <= _battlePoints)
        {
            float newValue = _battlePoints - cost;
            BattlePointsChanged?.Invoke(_battlePoints, newValue);
            RpcBattlePointsChanged(_battlePoints, newValue);
            _battlePoints = newValue;

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

    [ClientRpc]
    private void RpcCharacterSpawned(GameObject character)
    {
        var item = character.GetComponent<Character>();

        _characters.Add(item);
        CharacterSpawned?.Invoke(item);

        item.CharacterDied += OnCharacterDied;
    }
}
