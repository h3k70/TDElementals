using UnityEngine;

public interface IDamageDealer
{
    public Sprite Icon { get; }
    public Elements Element { get; }
    public GameObject Self { get; }
}
