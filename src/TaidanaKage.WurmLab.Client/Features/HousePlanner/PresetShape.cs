namespace TaidanaKage.WurmLab.Client.Features.HousePlanner;

/// <summary>
/// A named pre-defined shape that can be drawn onto the tile grid.
/// </summary>
internal sealed class PresetShape
{
    /// <summary>
    /// Display name shown in the dropdown, e.g. "8x8" or "Donut 7x7".
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Draws the shape onto an already cleared grid.
    /// </summary>
    public required Action Draw { get; init; }
}
