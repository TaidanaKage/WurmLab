namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

/// <summary>
/// An axis-aligned, inclusive rectangle of tiles, expressed in tile coordinates relative to the player.
/// Cardinal and diagonal directions produce a single tile; secondary intercardinal directions produce a run of tiles.
/// </summary>
internal readonly record struct TileRegion(sbyte MinX, sbyte MaxX, sbyte MinY, sbyte MaxY)
{
    /// <summary>
    /// Width of the region, in tiles.
    /// </summary>
    internal int WidthInTiles
    {
        get
        {
            return MaxX - MinX + 1;
        }
    }

    /// <summary>
    /// Height of the region, in tiles.
    /// </summary>
    internal int HeightInTiles
    {
        get
        {
            return MaxY - MinY + 1;
        }
    }

    /// <summary>
    /// Indicates whether the region is taller than it is wide, in which case labels must be rotated.
    /// </summary>
    internal bool IsVertical
    {
        get
        {
            return HeightInTiles > WidthInTiles;
        }
    }
}
