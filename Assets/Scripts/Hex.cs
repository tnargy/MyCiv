using UnityEngine;

public class Hex
{
    public Hex(int q, int r, float radius = 1f)
    {
        // q + r + s = 0
        // s = -(q + r)

        this.q = q;
        this.r = r;
        this.s = -(q + r);
        this.diameter = radius * 2;
        this.width = width_multiplier * diameter;
        this.Horz_spacing = width;
        this.Vert_spacing = diameter * 0.75f;
    }

    // Coordinates
    public readonly int q, r, s;
    private readonly float diameter, width;
    public float Elevation, Moisture;

    static readonly float width_multiplier = Mathf.Sqrt(3) / 2;

    public float Horz_spacing { get; }
    public float Vert_spacing { get; }

    public Vector3 Position()
    {
        return new Vector3(
            Horz_spacing * (q + r / 2f),
            0,
            Vert_spacing * r);
    }

    public static float Distance(Hex a, Hex b)
    {
        // FIXME: Wrapping
        return
            Mathf.Max(
                Mathf.Abs(a.q - b.q), 
                Mathf.Abs(a.r - b.r), 
                Mathf.Abs(a.s - b.s));
    }
}
