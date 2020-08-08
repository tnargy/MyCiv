public class Unit
{
    public string Name = "Unnamed";
    public int HP = 100, Strength = 8;
    public int Movement = 2;
    public int MovementRemainning = 2;

    public Hex Hex { get; protected set; }

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    public void SetHex(Hex newHex)
    {
        Hex oldHex = Hex;
        if (Hex != null)
            Hex.RemoveUnit(this);
        Hex = newHex;
        Hex.AddUnit(this);

        if (OnUnitMoved != null)
            OnUnitMoved(oldHex, newHex);
    }

    public void DoTurn()
    {
        Hex oldHex = Hex;
        Hex newHex = oldHex.HexMap.GetHexAt(oldHex.Q + 1, oldHex.R);

        SetHex(newHex);
    }
}
