using UnityEngine;
using UnityEngine.UI;

public class KillLogUI : MonoBehaviour
{
    [SerializeField] private Image _damageDealerImage;
    [SerializeField] private Image _damageTakerImage;

    public void Init(IDamageDealer damageDealer, IDamageable damageable)
    {
        _damageDealerImage.sprite = damageDealer.Icon;
        _damageTakerImage.sprite = damageable.Icon;
    }
}
