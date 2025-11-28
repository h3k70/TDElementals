using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectManagerUI : MonoBehaviour
{
    [SerializeField] private Dropdown _ipDropdown;
    [SerializeField] private Dropdown _modeDropdown;
    [SerializeField] private Button _startButton;

    private Dictionary<int, Servers> _dropdownToServer = new()
    {
        {0, Servers.Localhost},
        {1, Servers.MainServer},
    };

    private void Start()
    {
        _startButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        HTTPLibrary.GetIP(_dropdownToServer.GetValueOrDefault(_ipDropdown.value));
    }
}
