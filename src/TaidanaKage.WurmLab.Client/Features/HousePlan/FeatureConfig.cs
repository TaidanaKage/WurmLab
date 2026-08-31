namespace TaidanaKage.WurmLab.Client.Features.HousePlan;

/// <summary>
/// Constant configuration values for the House Plan feature.
/// </summary>
internal static class FeatureConfig
{
    /// <summary>
    /// Number of rows in the house plan grid (in tiles).
    /// </summary>
    internal const int GridRows = 20;

    /// <summary>
    /// Number of columns in the house plan grid (in tiles).
    /// </summary>
    internal const int GridColumns = 20;

    /// <summary>
    /// Size of a single tile in pixels.
    /// </summary>
    internal const int TileSize = 30;

    /// <summary>
    /// Maximum achievable skill value in the game (hardcoded game limit).
    /// </summary>
    internal const int MaxSkill = 100;
}
