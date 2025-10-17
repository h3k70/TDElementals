using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterSubUI : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private DecalProjector _selectProjector;
    [SerializeField] private PopText _popText;
    [SerializeField] private HPBarUI _hpBar;
    [SerializeField] private BuffIconUI _buffImagePref;
    [SerializeField] private GameObject _buffImageContainer;

    private List<IDisposable> _disposabls = new();
    private SelectCircleDisplay _selectCircleDisplay;
    private List<BuffIconUI> _buffImages = new();

    private void Awake()
    {
        _selectCircleDisplay = new(_selectProjector, _character);

        _disposabls.Add(_selectCircleDisplay);

        _character.DamageTaked += OnDamageTaked;
        _character.CharacterDied += OnDied;
        _character.BuffAdded += OnBuffAdded;
        _character.BuffRemoved += OnBuffRemoved;

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
        _character.CharacterDied -= OnDied;
        _character.BuffAdded -= OnBuffAdded;
        _character.BuffRemoved -= OnBuffRemoved;
    }

    private void OnDamageTaked(IDamageable damageable, float damage)
    {
        _popText.Show("-" + damage.ToString("0.0"));
    }

    private void OnDied(Character character)
    {
        gameObject.SetActive(false);
    }

    private void OnBuffAdded(Buffs buff)
    {
        var icon = Instantiate(_buffImagePref, _buffImageContainer.transform);
        _buffImages.Add(icon);
        icon.Init(buff);
    }

    private void OnBuffRemoved(Buffs buff)
    {
        var icon = _buffImages.FirstOrDefault(item => item.Buff == buff);
        _buffImages.Remove(icon);
        Destroy(icon.gameObject);
    }
}
