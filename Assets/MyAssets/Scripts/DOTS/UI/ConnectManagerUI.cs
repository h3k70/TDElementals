using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectManagerUI : MonoBehaviour
{
    [Scene]
    [SerializeField] private string _gameScene;
    [SerializeField] private TMP_Dropdown _ipDropdown;
    [SerializeField] private TMP_Dropdown _modeDropdown;
    [SerializeField] private Button _startButton;

    private ConnectManager _connectManager;
    private Dictionary<int, Servers> _dropdownToServer = new()
    {
        {0, Servers.Localhost},
        {1, Servers.MainServer},
    };
    private Dictionary<int, ConnectMode> _dropdownToMode = new()
    {
        {0, ConnectMode.Server},
        {1, ConnectMode.Client},
    };

    private void Start()
    {
        _startButton.onClick.AddListener(OnStart);
        _connectManager = new(SceneUtility.GetBuildIndexByScenePath(_gameScene));
    }

    private void OnStart()
    {
        switch (_dropdownToMode.GetValueOrDefault(_modeDropdown.value))
        {
            case ConnectMode.None:
                Debug.LogError("Error mode");
                break;

            case ConnectMode.Server:
                _connectManager.StartServerWorld(HTTPLibrary.Port);
                break;

            case ConnectMode.Client:
                _connectManager.StartClientWorld(HTTPLibrary.GetIP(_dropdownToServer.GetValueOrDefault(_ipDropdown.value)), HTTPLibrary.Port);
                break;

            case ConnectMode.ServerAndClient:
                _connectManager.StartServerWorld(HTTPLibrary.Port);
                _connectManager.StartClientWorld(HTTPLibrary.GetIP(_dropdownToServer.GetValueOrDefault(_ipDropdown.value)), HTTPLibrary.Port);
                break;
        }
    }
}
