using System;

public class ReactiveFloatVariable
{
    public event Action<float, float> Changed;

    private float _value;

    public ReactiveFloatVariable() : this(default(float))
    {

    }

    public ReactiveFloatVariable(float value)
    {
        _value = value;
    }

    public float Value
    {
        get => _value;
        set
        {
            float oldValue = _value;

            _value = value;

            if (oldValue != _value)
                Changed?.Invoke(oldValue, _value);
        }
    }
}
