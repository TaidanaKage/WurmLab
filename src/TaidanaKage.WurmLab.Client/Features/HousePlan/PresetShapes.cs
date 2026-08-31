namespace TaidanaKage.WurmLab.Client.Features.HousePlan;

/// <summary>
/// Catalog of pre-defined shapes for the House Plan feature.
/// </summary>
internal static class PresetShapes
{
    internal static readonly List<PresetShape> All = BuildCatalog();

    private static List<PresetShape> BuildCatalog()
    {
        List<PresetShape> catalog = [];

        // Solid squares from 2x2 to 9x9.
        for (int size = 2; size <= 9; size++)
        {
            // Capture the loop variable for the lambda.
            int s = size;

            catalog.Add(new PresetShape
            {
                Name = $"Solid {s}x{s}",
                Draw = () => DrawRectangle(s, s)
            });
        }

        // A long thin house, often used instead of a defensive wall.
        catalog.Add(new PresetShape
        {
            Name = "Wall 1x20",
            Draw = () => DrawRectangle(1, 20)
        });

        // Vertical variant of the wall.
        catalog.Add(new PresetShape
        {
            Name = "Wall 20x1",
            Draw = () => DrawRectangle(20, 1)
        });

        // Hollow squares ("donuts").

        catalog.Add(new PresetShape
        {
            Name = "Donut 4x4",
            Draw = () => DrawDonut(4)
        });

        catalog.Add(new PresetShape
        {
            Name = "Donut 5x5",
            Draw = () => DrawDonut(5)
        });

        catalog.Add(new PresetShape
        {
            Name = "Donut 6x6",
            Draw = () => DrawDonut(6)
        });

        catalog.Add(new PresetShape
        {
            Name = "Donut 7x7",
            Draw = () => DrawDonut(7)
        });

        catalog.Add(new PresetShape
        {
            Name = "Donut 8x8",
            Draw = () => DrawDonut(8)
        });

        catalog.Add(new PresetShape
        {
            Name = "Donut 9x9",
            Draw = () => DrawDonut(9)
        });

        // The whole grid.
        catalog.Add(new PresetShape
        {
            Name = "All",
            Draw = () => DrawRectangle(FeatureConfig.GridRows, FeatureConfig.GridColumns)
        });

        return catalog;
    }

    /// <summary>
    /// Draws a solid centered rectangle of the given dimensions.
    /// </summary>
    private static void DrawRectangle(int rows, int columns)
    {
        int startRow = (FeatureConfig.GridRows - rows) / 2;
        int startCol = (FeatureConfig.GridColumns - columns) / 2;

        for (int row = startRow; row < startRow + rows; row++)
        {
            for (int col = startCol; col < startCol + columns; col++)
            {
                FeatureState.Tiles[row, col] = true;
            }
        }
    }

    /// <summary>
    /// Draws a centered hollow square: a solid square with the interior removed.
    /// </summary>
    private static void DrawDonut(int size)
    {
        int startRow = (FeatureConfig.GridRows - size) / 2;
        int startCol = (FeatureConfig.GridColumns - size) / 2;

        for (int row = startRow; row < startRow + size; row++)
        {
            for (int col = startCol; col < startCol + size; col++)
            {
                // Enable only the outer ring; the interior stays empty.
                bool isEdge = row == startRow || row == startRow + size - 1
                    || col == startCol || col == startCol + size - 1;

                FeatureState.Tiles[row, col] = isEdge;
            }
        }
    }
}
