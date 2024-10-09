using System.Collections.Generic;
using UnityEngine.Events;

public class Listened<T> : UnityEvent<T>
{
    public Listened(params UnityAction<T>[] actions)
    {
        for (int i = 0; i < actions.Length; i++) AddListener(actions[i]);
    }

    T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value)) Invoke(value);
            _value = value;
        }
    }
}
