namespace TaidanaKage.WurmLab.Client.Features.HousePlanner;

/// <summary>
/// Calculation and state-manipulation helpers for the House Plan feature.
/// Operates directly on <see cref="FeatureState"/>.
/// </summary>
internal static class FeatureUtils
{
    internal static void Recalculate()
    {
        // Reset previous results.
        FeatureState.ValidationError = null;
        FeatureState.RequiredSkill = 0;
        FeatureState.RequiredMaterials = "";
        FeatureState.TileCount = 0;
        FeatureState.WallCount = 0;
        FeatureState.DetailedBreakdown = "";

        // Count the enabled tiles.
        int tileCount = CountEnabledTiles();

        if (tileCount == 0)
        {
            FeatureState.ValidationError = "The plan is empty. Enable at least one tile.";
            return;
        }

        if (!IsContinuous(tileCount))
        {
            FeatureState.ValidationError = "The shape is not continuous. All tiles must be connected.";
            return;
        }

        // Count the walls. Every edge of an enabled tile that borders a disabled tile
        // (or the grid boundary) requires a wall. This includes interior walls around
        // hollow areas (e.g. doughnut houses), which increases the required skill.
        int wallCount = CountWalls();

        // Formula: number of tiles + number of walls - 5.
        int requiredSkill = tileCount + wallCount - 5;

        // The game has a hardcoded skill cap of 100; larger plans are impossible to build.
        if (requiredSkill > 100)
        {
            FeatureState.ValidationError =
                $"The plan requires {requiredSkill} carpentry skill, but the maximum skill in the game is 100.";
            return;
        }

        // Store the counts so the UI can display them as fields.
        FeatureState.TileCount = tileCount;
        FeatureState.WallCount = wallCount;

        FeatureState.RequiredSkill = requiredSkill;

        // Calculate the material requirements for the currently selected house type.
        FeatureState.RequiredMaterials = BuildMaterialRequirements(FeatureState.SelectedHouseType, wallCount);

        // Build a plain-text explanation of the calculation.
        FeatureState.DetailedBreakdown =
            $"Formula: tiles + walls - 5\n" +
            $"Required skill: {tileCount} + {wallCount} - 5 = {FeatureState.RequiredSkill}";
    }

    /// <summary>
    /// Selects a solid square of the given size, centered in the grid.
    /// A resulting skill above the maximum is allowed; validation will report it.
    /// </summary>
    [Obsolete]
    internal static void SelectSquare(int squareSize)
    {
        // Clear the grid first (no recalculation yet).
        SetAllTilesCore(false);

        // Enable the centered square.
        int startRow = (FeatureConfig.GridRows - squareSize) / 2;
        int startCol = (FeatureConfig.GridColumns - squareSize) / 2;

        for (int row = startRow; row < startRow + squareSize; row++)
        {
            for (int col = startCol; col < startCol + squareSize; col++)
            {
                FeatureState.Tiles[row, col] = true;
            }
        }

        Recalculate();
    }

    [Obsolete]
    internal static void SelectAll()
    {
        SetAllTilesCore(true);
        Recalculate();
    }

    internal static void UnselectAll()
    {
        SetAllTilesCore(false);
        Recalculate();
    }

    /// <summary>
    /// Sets every tile to the given value without triggering a recalculation.
    /// </summary>
    private static void SetAllTilesCore(bool value)
    {
        for (int row = 0; row < FeatureConfig.GridRows; row++)
        {
            for (int col = 0; col < FeatureConfig.GridColumns; col++)
            {
                FeatureState.Tiles[row, col] = value;
            }
        }
    }

    internal static bool[,] CreateDefaultTiles()
    {
        // Start with all tiles disabled.
        bool[,] tiles = new bool[FeatureConfig.GridRows, FeatureConfig.GridColumns];

        // Toggle the tile in the middle of the grid by default.
        int middleRow = FeatureConfig.GridRows / 2;
        int middleColumn = FeatureConfig.GridColumns / 2;
        tiles[middleRow, middleColumn] = true;

        return tiles;
    }

    private static int CountEnabledTiles()
    {
        int count = 0;

        for (int row = 0; row < FeatureConfig.GridRows; row++)
        {
            for (int col = 0; col < FeatureConfig.GridColumns; col++)
            {
                if (FeatureState.Tiles[row, col])
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int CountWalls()
    {
        // Counts outer walls. In game terminology, every wall on the perimeter of the
        // building is an outer wall — including walls around hollow areas (e.g. doughnut
        // houses), since the hollow center is not part of the building.
        // Inner walls (dividing individual rooms) are a different concept and are not
        // relevant to this calculation.

        int walls = 0;

        for (int row = 0; row < FeatureConfig.GridRows; row++)
        {
            for (int col = 0; col < FeatureConfig.GridColumns; col++)
            {
                if (!FeatureState.Tiles[row, col])
                {
                    continue;
                }

                // Check all four neighbors; a wall is needed where there is no enabled neighbor.
                if (row == 0 || !FeatureState.Tiles[row - 1, col])
                {
                    walls++;
                }

                if (row == FeatureConfig.GridRows - 1 || !FeatureState.Tiles[row + 1, col])
                {
                    walls++;
                }

                if (col == 0 || !FeatureState.Tiles[row, col - 1])
                {
                    walls++;
                }

                if (col == FeatureConfig.GridColumns - 1 || !FeatureState.Tiles[row, col + 1])
                {
                    walls++;
                }
            }
        }

        return walls;
    }

    private static bool IsContinuous(int tileCount)
    {
        // Find any starting tile for the flood fill.
        int startRow = -1;
        int startCol = -1;

        for (int row = 0; row < FeatureConfig.GridRows && startRow < 0; row++)
        {
            for (int col = 0; col < FeatureConfig.GridColumns; col++)
            {
                if (FeatureState.Tiles[row, col])
                {
                    startRow = row;
                    startCol = col;
                    break;
                }
            }
        }

        // Flood fill (4-directional) from the starting tile.
        bool[,] visited = new bool[FeatureConfig.GridRows, FeatureConfig.GridColumns];
        Queue<(int Row, int Col)> queue = [];

        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol));

        int visitedCount = 0;

        while (queue.Count > 0)
        {
            (int row, int col) = queue.Dequeue();
            visitedCount++;

            // Visit the four orthogonal neighbors.
            (int Row, int Col)[] neighbors =
            [
                (row - 1, col),
                (row + 1, col),
                (row, col - 1),
                (row, col + 1)
            ];

            foreach ((int nRow, int nCol) in neighbors)
            {
                bool insideGrid = nRow >= 0 && nRow < FeatureConfig.GridRows && nCol >= 0 && nCol < FeatureConfig.GridColumns;

                if (insideGrid && FeatureState.Tiles[nRow, nCol] && !visited[nRow, nCol])
                {
                    visited[nRow, nCol] = true;
                    queue.Enqueue((nRow, nCol));
                }
            }
        }

        // The shape is continuous when the flood fill reached every enabled tile.
        return visitedCount == tileCount;
    }

    /// <summary>
    /// Clears the grid, draws the given preset shape, and recalculates.
    /// </summary>
    internal static void DrawPreset(PresetShape preset)
    {
        // Clear the grid first (no recalculation yet).
        SetAllTilesCore(false);

        preset.Draw();

        Recalculate();
    }

    /// <summary>
    /// Builds a plain-text list of material requirements for the given house type,
    /// based on the number of walls in the plan.
    /// </summary>
    private static string BuildMaterialRequirements(HouseType houseType, int wallCount)
    {
        List<string> lines = [];

        foreach (MaterialRequirement material in houseType.MaterialsPerWall)
        {
            // Total amount for the whole plan.
            int totalAmount = material.AmountPerWall * wallCount;

            lines.Add($"- {totalAmount} {material.Name}");
        }

        string result = string.Join("\n", lines);

        return result;
    }
}
