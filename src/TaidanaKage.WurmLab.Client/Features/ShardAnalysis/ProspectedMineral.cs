namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

internal enum ProspectedMineral : byte
{
    /// <summary>
    /// When player's prospecting skill is not high enough to determine the exact mineral.
    /// </summary>
    Something = 0,

    Iron = 1,
    Copper = 2,
    Tin = 3,
    Zinc = 4,
    Silver = 5,
    Gold = 6,
    Lead = 7,
    Marble = 8,
    Sandstone = 9,
    Slate = 10,
    RockSalt = 11,
    Glimmersteel = 12,
    Adamantine = 13,
    Seryll = 14,
}
