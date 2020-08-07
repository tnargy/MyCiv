using UnityEngine;

public class Hex
{
    public Hex(HexMap hexMap, int q, int r, float radius = 1f)
    {
        // q + r + s = 0
        // s = -(q + r)

        this.Q = q;
        this.R = r;
        this.S = -(q + r);
        this.diameter = radius * 2;
        this.width = width_multiplier * diameter;
        this.Horz_spacing = width;
        this.Vert_spacing = diameter * 0.75f;
        this.hexMap = hexMap;
    }


    public readonly int Q, R, S;    // Coordinates
    public float Elevation { get; set; }
    public float Moisture { get; set; }
    public float Horz_spacing { get; }
    public float Vert_spacing { get; }

    private readonly HexMap hexMap;
    private readonly float diameter, width;
    static readonly float width_multiplier = Mathf.Sqrt(3) / 2;

    public Vector3 Position()
    {
        return new Vector3(
            Horz_spacing * (Q + R / 2f),
            0,
            Vert_spacing * R);
    }

    public static float Distance(Hex a, Hex b)
    {
        int dQ = Mathf.Abs(a.Q - b.Q);
        if (dQ > a.hexMap.MapX / 2)
            dQ = a.hexMap.MapX - dQ;

        return
            Mathf.Max(
                dQ,
                Mathf.Abs(a.R - b.R),
                Mathf.Abs(a.S - b.S));
    }
}
