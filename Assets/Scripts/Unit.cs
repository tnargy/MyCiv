using UnityEngine;

public class Unit
{
    public string Name = "Unnamed";
    public int HP = 100, Strength = 8;
    public int Movement = 2;
    public int MovementRemainning = 2;

    public Hex Hex { get; protected set; }

    public void SetHex(Hex hex)
    {
        if (Hex != null)
            Hex.RemoveUnit(this);
        Hex = hex;
        Hex.AddUnit(this);
    }

    public void Turn()
    {

    }
}
