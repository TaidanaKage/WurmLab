namespace TaidanaKage.WurmLab.Client.Features.HousePlan;

/// <summary>
/// A very simple state management for the House Plan feature.
/// </summary>
internal static class FeatureState
{
    internal static bool[,] Tiles = FeatureUtils.CreateDefaultTiles();

    internal static int RequiredSkill { get; set; } = 0;

    internal static string RequiredMaterials { get; set; } = "";

    /// <summary>
    /// Number of enabled tiles in the current plan.
    /// </summary>
    internal static int TileCount { get; set; } = 0;

    /// <summary>
    /// Number of walls required by the current plan.
    /// </summary>
    internal static int WallCount { get; set; } = 0;

    /// <summary>
    /// Plain-text explanation of how the skill value was calculated.
    /// </summary>
    internal static string DetailedBreakdown { get; set; } = "";

    /// <summary>
    /// Error message describing why the plan is invalid, or null when the plan is valid.
    /// </summary>
    internal static string? ValidationError { get; set; }

    /// <summary>
    /// The preset shape currently selected in the drawing tools.
    /// Defaults to the 8x8 solid square preset.
    /// </summary>
    internal static PresetShape SelectedPreset { get; set; } = PresetShapes.All.First(p => p.Name == "Solid 8x8");

    /// <summary>
    /// The house type currently selected in the requirements section.
    /// Defaults to the first entry in the catalog.
    /// </summary>
    internal static HouseType SelectedHouseType { get; set; } = HouseTypes.All.First();
}
