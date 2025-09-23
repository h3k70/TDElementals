using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseSubUI : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private PopText _popText;
    [SerializeField] private BarUI _hpBar;

    private void Awake()
    {
        _base.DamageTaked += OnDamageTaked;

        _hpBar.Init(_base);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    private void OnDamageTaked(IDamageable damageable, float damage)
    {
        _popText.Show("-" + damage);
    }
}
