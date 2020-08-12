public class City : MapObject
{
    public City(Hex hex, string name)
    {
        this.Hex = hex;
        Name = name;
    }
}