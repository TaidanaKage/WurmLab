namespace TaidanaKage.WurmLab.Client.Features.SurfaceProspecting;

/// <summary>
/// Must be public, otherwise Blazor components cannot use it as a parameter, and so on.
/// </summary>
public sealed class TileModel
{
    internal int X { get; set; }

    internal int Y { get; set; }

    internal int Certainty { get; set; }

    internal string? PrimaryMineral { get; set; }

    internal bool IsProspected { get; set; }
}
