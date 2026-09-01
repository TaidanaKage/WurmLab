namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

/// <summary>
/// Cardinal and diagonal directions are precise: they form exact 1-tile vectors.
/// Secondary intercardinal directions represent imprecise angular cones spanning multiple potential target tiles.
/// </summary>
internal enum ProspectedDirection : byte
{
    // 4 cardinal directions
    North = 1,
    East = 2,
    South = 3,
    West = 4,

    // 4 diagonal (ordinal) directions
    NorthEast = 5,
    SouthEast = 6,
    SouthWest = 7,
    NorthWest = 8,

    // 8 secondary intercardinal directions (between cardinal and diagonal)
    EastOfNorth = 9,
    WestOfNorth = 10,
    NorthOfEast = 11,
    SouthOfEast = 12,
    EastOfSouth = 13,
    WestOfSouth = 14,
    NorthOfWest = 15,
    SouthOfWest = 16
}
