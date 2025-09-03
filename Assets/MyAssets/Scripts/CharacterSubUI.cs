using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterSubUI : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private DecalProjector _selectProjector;

    private List<IDisposable> _disposabls = new();
    private SelectCircleDisplay _selectCircleDisplay;

    private void Awake()
    {
        _selectCircleDisplay = new(_selectProjector, _character);

        _disposabls.Add(_selectCircleDisplay);
    }
}
