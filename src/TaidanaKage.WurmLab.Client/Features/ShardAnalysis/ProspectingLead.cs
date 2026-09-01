namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

/// <summary>
/// Defines a single lead found in a prospecting perimeter.
/// </summary>
internal sealed class ProspectingLead
{
    /// <summary>
    /// Each lead has a specific direction assigned.
    /// </summary>
    internal required ProspectedDirection Direction { get; init; }

    /// <summary>
    /// Each lead may have a specific mineral assigned (including the "Something" option).
    /// NULL, if this lead doesn't contain any mineral info (for example, if only Flint or Salt was detected).
    /// </summary>
    internal required ProspectedMineral? Mineral { get; init; }

    /// <summary>
    /// Each lead may have a specific quality assigned.
    /// NULL, if the player wasn't able to determine the quality.
    /// </summary>
    internal required ProspectedQuality? Quality { get; init; }

    /// <summary>
    /// Indicates whether this lead contains Flint detection.
    /// This is completely independent of mineral detection.
    /// </summary>
    internal bool HasFlint { get; init; } = false;

    /// <summary>
    /// Indicates whether this lead contains Salt detection.
    /// This is completely independent of mineral detection.
    /// Note: Salt and Rock salt (mineral) are two completely different concepts.
    /// </summary>
    internal bool HasSalt { get; init; } = false;
}
