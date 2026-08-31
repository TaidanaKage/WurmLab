namespace TaidanaKage.WurmLab.Client.Features.HousePlan;

/// <summary>
/// Catalog of pre-defined house types for the House Plan feature.
/// </summary>
internal static class HouseTypes
{
    internal static readonly List<HouseType> All =
    [
        new HouseType
        {
            Name = "Wooden",
            MaterialsPerWall =
            [
                new MaterialRequirement { Name = "large nail", AmountPerWall = 1 },
                new MaterialRequirement { Name = "plank", AmountPerWall = 20 }
            ]
        },

        new HouseType
        {
            Name = "Brick",
            MaterialsPerWall =
            [
                new MaterialRequirement { Name = "mortar", AmountPerWall = 20 },
                new MaterialRequirement { Name = "brick", AmountPerWall = 20 }
            ]
        },

        new HouseType
        {
            Name = "Timber framed",
            MaterialsPerWall =
            [
                new MaterialRequirement { Name = "wooden beam", AmountPerWall = 5 },
                new MaterialRequirement { Name = "clay", AmountPerWall = 20 },
                new MaterialRequirement { Name = "mixed grass", AmountPerWall = 10 }
            ]
        }
    ];
}
