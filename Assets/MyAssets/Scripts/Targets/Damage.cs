using System.Collections.Generic;

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
        /*
        switch (damageDealer.Element)
        {
            case Elements.None:
                break;

            case Elements.Air:

                if (damageable.Element == Elements.Water)
                    Value *= _multipler;
                else if (damageable.Element == Elements.Earth)
                    Value /= _multipler;
                break;

            case Elements.Fire:

                if (damageable.Element == Elements.Earth)
                    Value *= _multipler;
                else if (damageable.Element == Elements.Water)
                    Value /= _multipler;
                break;

            case Elements.Water:

                if (damageable.Element == Elements.Fire)
                    Value *= _multipler;
                else if (damageable.Element == Elements.Air)
                    Value /= _multipler;
                break;

            case Elements.Earth:

                if (damageable.Element == Elements.Air)
                    Value *= _multipler;
                else if (damageable.Element == Elements.Fire)
                    Value /= _multipler;
                break;

            default:
                break;
        }*/

        damageable.TakeDamage(this);
    }
}

public class ElementsCross
{
    public static readonly Dictionary<Elements, Elements> StrongWeak = new Dictionary<Elements, Elements>()
    {
        { Elements.Air, Elements.Water },
        { Elements.Earth, Elements.Air },
        { Elements.Fire, Elements.Earth },
        { Elements.Water, Elements.Fire },
    };
}