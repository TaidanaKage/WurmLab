namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

/// <summary>
/// Represents all leads that share the same perimeter distance and the same direction,
/// and therefore occupy the very same region of the map.
/// </summary>
internal sealed class ProspectingSlot
{
    /// <summary>
    /// Distance in tiles from the player (1 to 6).
    /// </summary>
    internal required byte Distance { get; init; }

    /// <summary>
    /// The direction shared by all leads in this slot.
    /// </summary>
    internal required ProspectedDirection Direction { get; init; }

    /// <summary>
    /// All leads found at this distance in this direction.
    /// A single direction may carry several leads, for example a mineral lead plus a salt lead.
    /// </summary>
    internal List<ProspectingLead> Leads { get; init; } = [];
}
