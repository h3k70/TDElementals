using UnityEngine;

public class BaseSubUI : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private PopText _popText;
    [SerializeField] private HPBarUI _hpBar;

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
        _popText.Show("-" + damage.ToString("0.0"));
    }
}
