using UnityEngine;

public struct Damage
{
    public float Value;
    public IDamageDealer DamageDealer;
    public IDamageable Damageable;

    public Damage(float value, IDamageDealer damageDealer, IDamageable damageable)
    {
        float _multipler = 1.3f;

        Value = value;
        DamageDealer = damageDealer;
        Damageable = damageable;

        switch (damageDealer.Element)
        {
            case Elements.None:
                break;

            case Elements.Air:

                if (damageable.Element == Elements.Water)
                    value *= _multipler;
                else if (damageable.Element == Elements.Earth)
                    value /= _multipler;
                break;

            case Elements.Fire:

                if (damageable.Element == Elements.Earth)
                    value *= _multipler;
                else if (damageable.Element == Elements.Water)
                    value /= _multipler;
                break;

            case Elements.Water:

                if (damageable.Element == Elements.Fire)
                    value *= _multipler;
                else if (damageable.Element == Elements.Air)
                    value /= _multipler;
                break;

            case Elements.Earth:

                if (damageable.Element == Elements.Air)
                    value *= _multipler;
                else if (damageable.Element == Elements.Fire)
                    value /= _multipler;
                break;

            default:
                break;
        }

        damageable.TakeDamage(this);
    }
}