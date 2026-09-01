namespace TaidanaKage.WurmLab.Client.Features.HousePlanner;

/// <summary>
/// A single material and the amount of it required per wall.
/// </summary>
internal sealed class MaterialRequirement
{
    /// <summary>
    /// Display name of the material (e.g. "plank").
    /// </summary>
    internal required string Name { get; init; }

    /// <summary>
    /// Amount of the material required per wall.
    /// </summary>
    internal required int AmountPerWall { get; init; }
}
