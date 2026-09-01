namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

/// <summary>
/// Translates a prospected direction at a given distance into the region of tiles it covers.
/// The 16 directions partition the perimeter ring: no two directions ever overlap.
/// </summary>
internal static class DirectionGeometry
{
    /// <summary>
    /// Resolves the tile region covered by a direction at a given distance.
    /// </summary>
    /// <param name="direction">The prospected direction.</param>
    /// <param name="distance">Distance in tiles from the player.</param>
    /// <returns>
    /// The covered region, or null when the direction covers no tiles at this distance
    /// (secondary intercardinal directions are empty at distance 1).
    /// </returns>
    internal static TileRegion? Resolve(ProspectedDirection direction, byte distance)
    {
        sbyte far = (sbyte)distance;
        sbyte near = (sbyte)(distance - 1);

        // Secondary intercardinal directions span the tiles strictly between a cardinal and a diagonal,
        // so they cover nothing at all when the perimeter is only one tile away.
        bool isSecondary = direction >= ProspectedDirection.EastOfNorth;
        if (isSecondary && near < 1)
        {
            return null;
        }

        switch (direction)
        {
            // Cardinal directions: the midpoint of an edge.
            case ProspectedDirection.North:
                return new TileRegion(0, 0, far, far);
            case ProspectedDirection.East:
                return new TileRegion(far, far, 0, 0);
            case ProspectedDirection.South:
                return new TileRegion(0, 0, (sbyte)(-far), (sbyte)(-far));
            case ProspectedDirection.West:
                return new TileRegion((sbyte)(-far), (sbyte)(-far), 0, 0);

            // Diagonal directions: a corner of the ring.
            case ProspectedDirection.NorthEast:
                return new TileRegion(far, far, far, far);
            case ProspectedDirection.SouthEast:
                return new TileRegion(far, far, (sbyte)(-far), (sbyte)(-far));
            case ProspectedDirection.SouthWest:
                return new TileRegion((sbyte)(-far), (sbyte)(-far), (sbyte)(-far), (sbyte)(-far));
            case ProspectedDirection.NorthWest:
                return new TileRegion((sbyte)(-far), (sbyte)(-far), far, far);

            // Secondary directions on the north edge: horizontal runs.
            case ProspectedDirection.EastOfNorth:
                return new TileRegion(1, near, far, far);
            case ProspectedDirection.WestOfNorth:
                return new TileRegion((sbyte)(-near), -1, far, far);

            // Secondary directions on the south edge: horizontal runs.
            case ProspectedDirection.EastOfSouth:
                return new TileRegion(1, near, (sbyte)(-far), (sbyte)(-far));
            case ProspectedDirection.WestOfSouth:
                return new TileRegion((sbyte)(-near), -1, (sbyte)(-far), (sbyte)(-far));

            // Secondary directions on the east edge: vertical runs.
            case ProspectedDirection.NorthOfEast:
                return new TileRegion(far, far, 1, near);
            case ProspectedDirection.SouthOfEast:
                return new TileRegion(far, far, (sbyte)(-near), -1);

            // Secondary directions on the west edge: vertical runs.
            case ProspectedDirection.NorthOfWest:
                return new TileRegion((sbyte)(-far), (sbyte)(-far), 1, near);
            case ProspectedDirection.SouthOfWest:
                return new TileRegion((sbyte)(-far), (sbyte)(-far), (sbyte)(-near), -1);

            default:
                return null;
        }
    }
}
