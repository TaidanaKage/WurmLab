namespace TaidanaKage.WurmLab.Client.Features.HousePlanner;

/// <summary>
/// A buildable house type with its per-wall material requirements.
/// </summary>
internal sealed class HouseType
{
    /// <summary>
    /// Display name of the house type (e.g. "Wooden").
    /// </summary>
    internal required string Name { get; init; }

    /// <summary>
    /// Materials required to build a single wall.
    /// Key: material name, value: amount per wall.
    /// </summary>
    internal required List<MaterialRequirement> MaterialsPerWall { get; init; }
}
