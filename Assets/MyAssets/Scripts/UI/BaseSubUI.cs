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

    private void OnDamageTaked(Damage damage)
    {
        _popText.Show("-" + damage.Value.ToString("0.0"));
    }
}
