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
        _base2 = CreateBase(player2, _baseSpawnPoint2, _paths2);

        RpcStartGame(_base1.gameObject, _base2.gameObject);

        _base1.Destroed += OnBaseDestroed;
        _base2.Destroed += OnBaseDestroed;
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

        _gameplayUI.UnitPanelUI.Init(_ownerBase.CharactersPrefabs);
        _gameplayUI.UnitPanelUI.SelectedCharacterCard += OnSelectedCharacterCard;

        _gameplayUI.SwichLeftPathButton.onClick.AddListener(OnSwichLeftPath);
        _gameplayUI.SwichRightPathButton.onClick.AddListener(OnSwichRightPath);

        _ownerBase.BattlePointsChanged += OnBattlePointsChanged;
    }

    private Base CreateBase(NetworkConnectionToClient playerConn, Transform spawnPoint, List<Path> paths)
    {
        var tempBase = Instantiate(_basePref, spawnPoint);
        tempBase.Init(paths);
        NetworkServer.Spawn(tempBase.gameObject, playerConn);
        return tempBase;
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
        if (_ownerBase.SelectedForSpawnUnit == card.Character)
        {
            _ownerBase.TrySpawnUnit(_ownerBase.SelectedForSpawnUnit);
        }

        _ownerBase.SelectedForSpawnUnit = card.Character;
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
            NetworkServer.UnSpawn(unit.gameObject);
        }
        foreach (var unit in _base2.Units)
        {
            NetworkServer.UnSpawn(unit.gameObject);
        }

        NetworkServer.UnSpawn(_base1.gameObject);
        NetworkServer.UnSpawn(_base2.gameObject);

        RpcResetAll();
    }

    private void OnSwichRightPath()
    {
        _ownerBase.SelecRightPath();
    }

    private void OnSwichLeftPath()
    {
        _ownerBase.SelectLeftPath();
    }

    private void OnBattlePointsChanged(float arg1, float arg2)
    {
        _gameplayUI.OnBattlePointsChanged(arg1, arg2);
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
            path.Select(false);

        foreach(var path in _paths2)
            path.Select(false);

        _gameplayUI.UnitPanelUI.SelectedCharacterCard -= OnSelectedCharacterCard;
        _ownerBase.BattlePointsChanged -= OnBattlePointsChanged;
        _gameplayUI.ResetAll();
    }
}
