using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Zenject;

public class Game : NetworkBehaviour
{
    public static Game Instance;

    [SerializeField] private Base _basePref;
    [SerializeField] private Transform _baseSpawnPoint1;
    [SerializeField] private Transform _baseCameraPoint1;
    [SerializeField] private List<Path> _paths1;
    [SerializeField] private Transform _baseSpawnPoint2;
    [SerializeField] private Transform _baseCameraPoint2;
    [SerializeField] private List<Path> _paths2;
    [Space(25)]
    [SerializeField] private GameplayUI _gameplayUI;

    private Base _base1;
    private Base _base2;
    private Base _ownerBase;
    private NetworkConnectionToClient player1;
    private NetworkConnectionToClient player2;
    private Selector _selector;
    private float _expForKill = 3;

    public Base OwnerBase { get => _ownerBase; }

    [Inject]
    public void Inject(Selector selector)
    {
        _selector = selector;
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _gameplayUI.UnitPanelUI.SelectedCharacterCard -= OnSelectedCharacterCard;
    }

    public void AddPlayer(NetworkConnectionToClient conn)
    {
        if (player1 == null)
        {
            player1 = conn;
        }
        else if (player2 == null)
        {
            player2 = conn;
            StartGame();
        }
        else
            Debug.LogError("RoomFull");
    }

    private void StartGame()
    {
        _base1 = CreateBase(player1, _baseSpawnPoint1, _paths1);
        CreateUnitsForCard(_base1);
        _base2 = CreateBase(player2, _baseSpawnPoint2, _paths2);
        CreateUnitsForCard(_base2);

        RpcStartGame(_base1.gameObject, _base2.gameObject);

        _base1.Destroed += OnBaseDestroed;
        _base2.Destroed += OnBaseDestroed;

        _base1.CharacterSpawned += OnCharacterSpawned;
        _base2.CharacterSpawned += OnCharacterSpawned;
    }


    [ClientRpc]
    private void RpcStartGame(GameObject base1, GameObject base2)
    {
        _base1 = base1.GetComponent<Base>();
        _base1.Init(_paths1);
        _base2 = base2.GetComponent<Base>();
        _base2.Init(_paths2);

        MarkLayerObj(_base1);
        MarkLayerObj(_base2);
        SetCameraPositionBehindOwnedBase();
        SetOwnerBase();

        _gameplayUI.UnitPanelUI.Init(_ownerBase.CharactersForCards, _ownerBase.CharactersPrefabs);
        _gameplayUI.UnitPanelUI.SelectedCharacterCard += OnSelectedCharacterCard;

        _gameplayUI.SwichLeftPathButton.onClick.AddListener(OnSwichLeftPath);
        _gameplayUI.SwichRightPathButton.onClick.AddListener(OnSwichRightPath);

        foreach (var item in _gameplayUI.BuffButtons)
            item.Init(_ownerBase);

        _ownerBase.BattlePointsChanged += OnBattlePointsChanged;
        OnBattlePointsChanged(_ownerBase.BattlePoints, _ownerBase.BattlePoints);

        _selector.SubSelected += OnSubSelected;
    }

    private Base CreateBase(NetworkConnectionToClient playerConn, Transform spawnPoint, List<Path> paths)
    {
        var tempBase = Instantiate(_basePref, spawnPoint);
        tempBase.Init(paths);
        NetworkServer.Spawn(tempBase.gameObject, playerConn);
        return tempBase;
    }

    private void CreateUnitsForCard(Base base1)
    {
        foreach (var prefab in base1.CharactersPrefabs)
        {
            var unit = Instantiate(prefab, new Vector3(999,999,999), Quaternion.identity);
            unit.gameObject.SetActive(false);
            NetworkServer.Spawn(unit.gameObject, base1.gameObject);
            base1.CharactersForCards.Add(unit);
            RpcCharactersForCardsAdd(unit.gameObject, base1.gameObject);
        }
    }

    private void MarkLayerObj(NetworkBehaviour obj)
    {
        if (obj.isOwned)
            obj.gameObject.layer = Layers.Ally;
        else
            obj.gameObject.layer = Layers.Enemy;
    }

    private void SetCameraPositionBehindOwnedBase()
    {
        if (_base1.isOwned)
            Camera.main.transform.SetPositionAndRotation(_baseCameraPoint1.position, _baseCameraPoint1.rotation);
        else
            Camera.main.transform.SetPositionAndRotation(_baseCameraPoint2.position, _baseCameraPoint2.rotation);
    }

    private void SetOwnerBase()
    {
        if (_base1.isOwned)
            _ownerBase = _base1;
        else
            _ownerBase = _base2;
    }

    private void OnSelectedCharacterCard(CharacterCardUI card)
    {
        if (_ownerBase.SelectedForSpawnUnit == card.CharacterPref)
        {
            _ownerBase.TrySpawnUnit(_ownerBase.SelectedForSpawnUnit, card.Character.Cost);
        }

        _ownerBase.SelectedForSpawnUnit = card.CharacterPref;
        _ownerBase.SelectedCardUnit = card.Character;
    }

    private void OnSubSelected(ISelectable selectable)
    {
        if (selectable is Path path && _selector.CurrentSelectablsUnit.Count > 0)
        {
            foreach (var item in _selector.CurrentSelectablsUnit)
            {
                if (item.Self.TryGetComponent(out Character character))
                {
                    character.SetPath(path);
                }
            }
        }
    }

    private void OnBaseDestroed()
    {
        GameEnd();
    }

    private void GameEnd()
    {
        ResetAll();

        StartGame();
    }

    private void ResetAll()
    {
        _base1.Destroed -= OnBaseDestroed;
        _base2.Destroed -= OnBaseDestroed;

        foreach (var unit in _base1.Units)
        {
            if (unit != null)
                NetworkServer.UnSpawn(unit.gameObject);
        }
        foreach (var unit in _base2.Units)
        {
            if (unit != null)
                NetworkServer.UnSpawn(unit.gameObject);
        }

        NetworkServer.UnSpawn(_base1.gameObject);
        NetworkServer.UnSpawn(_base2.gameObject);

        RpcResetAll();
    }

    private void OnSwichRightPath()
    {
        if (_selector.CurrentSelectablsUnit.Count > 0 && _selector.CurrentSelectablsUnit[0] is Character character)
        {
            character.SetPath(_ownerBase.RightPath);
        }
        else
        {
            _ownerBase.SelecRightPath();
        }
    }

    private void OnSwichLeftPath()
    {
        if (_selector.CurrentSelectablsUnit.Count > 0 && _selector.CurrentSelectablsUnit[0] is Character character)
        {
            character.SetPath(_ownerBase.LeftPath);
        }
        else
        {
            _ownerBase.SelectLeftPath();
        }
    }

    private void OnBattlePointsChanged(float arg1, float arg2)
    {
        _gameplayUI.OnBattlePointsChanged(arg1, arg2);
    }

    private void OnCharacterSpawned(Character character)
    {
        character.Died += OnCharacterDied;
    }

    private void OnCharacterDied(Damage damage)
    {
        var damagavle = damage.Damageable.Self.GetComponent<Character>();
        damagavle.Died -= OnCharacterDied;
        var character = damage.DamageDealer.Self.GetComponent<Character>();
        character.SelfCard.AddExp(damagavle.CurrentLVL + _expForKill);
    }

    [ClientRpc]
    public void RpcMarkLayerObj(NetworkBehaviour obj)
    {
        MarkLayerObj(obj);
    }

    [ClientRpc]
    public void RpcResetAll()
    {
        foreach(var path in _paths1)
            path.SetSelect(false);

        foreach(var path in _paths2)
            path.SetSelect(false);

        _gameplayUI.UnitPanelUI.SelectedCharacterCard -= OnSelectedCharacterCard;
        _ownerBase.BattlePointsChanged -= OnBattlePointsChanged;
        _gameplayUI.ResetAll();

        _selector.SubSelected -= OnSubSelected;
    }

    [ClientRpc]
    private void RpcCharactersForCardsAdd(GameObject character, GameObject base1)
    {
        base1.GetComponent<Base>().CharactersForCards.Add(character.GetComponent<Character>());
        character.gameObject.SetActive(false);
    }
}
