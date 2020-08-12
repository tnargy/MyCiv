public class MapObject
{
    public string Name;
    public int HP, Strength;
    public delegate void ObjectMovedDelegate(Hex oldHex, Hex newHex);
    public event ObjectMovedDelegate OnObjectMoved;

    public Hex Hex { get; protected set; }

    virtual public void SetHex(Hex newHex)
    {
        Hex oldHex;
        if (Hex != null)
        {
            oldHex = Hex;
        }
        else
        {
            oldHex = newHex;
        }
        Hex = newHex;

        OnObjectMoved?.Invoke(oldHex, newHex);
    }

}