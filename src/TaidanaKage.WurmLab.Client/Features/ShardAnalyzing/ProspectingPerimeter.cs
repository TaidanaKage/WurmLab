namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

/// <summary>
/// Defines a single square perimeter of tiles at a fixed distance from player. 
/// </summary>
internal sealed class ProspectingPerimeter
{
    /// <summary>
    /// Distance in tiles from player (1 to 6).
    /// </summary>
    internal required byte Distance { get; init; }

    /// <summary>
    /// List of leads found in this perimeter.
    /// The order is not meaningful.
    /// The list may include multiple leads for a single direction (e.g. one for a mineral, other for flint).
    /// </summary>
    internal List<ProspectingLead> Leads { get; init; } = [];
}
