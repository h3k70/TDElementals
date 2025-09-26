using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterSubUI : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private DecalProjector _selectProjector;
    [SerializeField] private PopText _popText;
    [SerializeField] private BarUI _hpBar;

    private List<IDisposable> _disposabls = new();
    private SelectCircleDisplay _selectCircleDisplay;

    private void Awake()
    {
        _selectCircleDisplay = new(_selectProjector, _character);

        _disposabls.Add(_selectCircleDisplay);

        _character.DamageTaked += OnDamageTaked;
        _character.Died += OnDied;

        _hpBar.Init(_character);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    private void OnDestroy()
    {
        foreach (var item in _disposabls)
            item.Dispose();

        _character.DamageTaked -= OnDamageTaked;
        _character.Died -= OnDied;
    }

    private void OnDamageTaked(IDamageable damageable, float damage)
    {
        _popText.Show("-" + damage);
    }

    private void OnDied(Damage damage)
    {
        gameObject.SetActive(false);
    }

}
