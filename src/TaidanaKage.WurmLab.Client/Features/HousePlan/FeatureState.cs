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
    /// Plain-text explanation of how the skill value was calculated.
    /// </summary>
    internal static string DetailedBreakdown { get; set; } = "";

    /// <summary>
    /// Error message describing why the plan is invalid, or null when the plan is valid.
    /// </summary>
    internal static string? ValidationError { get; set; }
}
