using TaidanaKage.WurmLab.Client.Common;

namespace TaidanaKage.WurmLab.Client.Features.SettlementUpkeep;

/// <summary>
/// A very simple state management for the Settlement Upkeep feature.
/// Values are kept in memory for the lifetime of the WebAssembly application,
/// so navigating away from the page and back again preserves the user's input.
/// </summary>
internal static class FeatureState
{
    // The cost of purchasing a single deed tile.
    private static readonly Money DeedTilePurchaseCost = Money.FromCoins(copper: 1);

    // The monthly upkeep of a single deed tile.
    private static readonly Money DeedTileUpkeepCost = Money.FromCoins(iron: 20);

    // The cost of purchasing a single perimeter tile.
    private static readonly Money PerimeterTilePurchaseCost = Money.FromCoins(iron: 50);

    // The monthly upkeep of a single perimeter tile.
    private static readonly Money PerimeterTileUpkeepCost = Money.FromCoins(iron: 5);

    // Settlement size, in tiles, per direction.
    internal static int North { get; set; } = 10;
    internal static int South { get; set; } = 10;
    internal static int East { get; set; } = 10;
    internal static int West { get; set; } = 10;

    // Perimeter size, in tiles.
    internal static int Perimeter { get; set; } = 5;

    // Number of hired templars.
    internal static int Templars { get; set; } = 0;

    // Whether the settlement is on an Epic cluster server.
    internal static bool IsEpic { get; set; } = false;

    // Calculated values.
    internal static Money InitialCost { get; private set; } = Money.Zero;
    internal static Money MonthlyUpkeep { get; private set; } = Money.Zero;

    // Calculated tile counts, useful for display and for the cost calculation itself.
    internal static long DeedTiles { get; private set; }
    internal static long PerimeterTiles { get; private set; }

    // Deed dimensions, in tiles, including the token tile.
    internal static long DeedWidth { get; private set; }
    internal static long DeedHeight { get; private set; }

    // Individual parts of the initial cost, shown in the detailed breakdown.
    internal static Money DeedPurchaseCost { get; private set; } = Money.Zero;
    internal static Money PerimeterPurchaseCost { get; private set; } = Money.Zero;
    internal static Money TemplarSummoningTotal { get; private set; } = Money.Zero;

    // Individual parts of the monthly upkeep, shown in the detailed breakdown.
    internal static Money DeedUpkeep { get; private set; } = Money.Zero;
    internal static Money PerimeterUpkeep { get; private set; } = Money.Zero;
    internal static Money TemplarUpkeepTotal { get; private set; } = Money.Zero;

    // Unit prices, exposed so the breakdown can explain the numbers it shows.
    internal static Money DeedTilePurchaseUnitCost => DeedTilePurchaseCost;
    internal static Money DeedTileUpkeepUnitCost => DeedTileUpkeepCost;
    internal static Money PerimeterTilePurchaseUnitCost => PerimeterTilePurchaseCost;
    internal static Money PerimeterTileUpkeepUnitCost => PerimeterTileUpkeepCost;
    internal static Money TemplarSummoningUnitCost => IsEpic ? TemplarSummoningCostEpic : TemplarSummoningCost;

    // The one-off cost of summoning a single templar on a normal server.
    private static readonly Money TemplarSummoningCost = Money.FromCoins(silver: 2);

    // The one-off cost of summoning a single templar on an Epic server.
    private static readonly Money TemplarSummoningCostEpic = Money.FromCoins(silver: 3);

    // The monthly upkeep of a single templar on a normal server.
    private static readonly Money TemplarUpkeepCost = Money.FromCoins(silver: 1);

    // On Epic, the monthly upkeep starts at 1 silver for the first templar and
    // grows by half a silver with every additional one.
    private static readonly Money TemplarUpkeepCostEpicBase = Money.FromCoins(silver: 1);
    private static readonly Money TemplarUpkeepCostEpicIncrement = Money.FromCoins(copper: 50);

    /// <summary>
    /// Recalculates the deed and perimeter costs from the current user input.
    /// </summary>
    internal static void Recalculate()
    {
        // The deed area includes the token tile itself, hence the "+ 1" in both dimensions.
        long width = (long)West + East + 1;
        long height = (long)North + South + 1;

        DeedWidth = width;
        DeedHeight = height;
        DeedTiles = width * height;

        // The perimeter is a band of the given width around the whole deed area.
        long outerWidth = width + (2L * Perimeter);
        long outerHeight = height + (2L * Perimeter);

        PerimeterTiles = (outerWidth * outerHeight) - DeedTiles;

        // The initial cost is the purchase price of every deed and perimeter tile,
        // plus the one-off summoning cost of every templar.
        DeedPurchaseCost = DeedTilePurchaseCost * DeedTiles;
        PerimeterPurchaseCost = PerimeterTilePurchaseCost * PerimeterTiles;
        TemplarSummoningTotal = CalculateTemplarSummoningCost();

        InitialCost = DeedPurchaseCost + PerimeterPurchaseCost + TemplarSummoningTotal;

        // The monthly upkeep is the upkeep of every deed and perimeter tile,
        // plus the monthly cost of every templar.
        DeedUpkeep = DeedTileUpkeepCost * DeedTiles;
        PerimeterUpkeep = PerimeterTileUpkeepCost * PerimeterTiles;
        TemplarUpkeepTotal = CalculateTemplarUpkeepCost();

        MonthlyUpkeep = DeedUpkeep + PerimeterUpkeep + TemplarUpkeepTotal;
    }

    /// <summary>
    /// Calculates the total one-off summoning cost of all hired templars.
    /// </summary>
    /// <returns>The summoning cost.</returns>
    private static Money CalculateTemplarSummoningCost()
    {
        // Every templar costs the same to summon; only the server type changes the price.
        Money costPerTemplar = IsEpic ? TemplarSummoningCostEpic : TemplarSummoningCost;

        return costPerTemplar * Templars;
    }

    /// <summary>
    /// Calculates the total monthly upkeep of all hired templars.
    /// </summary>
    /// <returns>The monthly templar upkeep.</returns>
    private static Money CalculateTemplarUpkeepCost()
    {
        // On a normal server every templar costs the same amount each month.
        if (!IsEpic)
        {
            return TemplarUpkeepCost * Templars;
        }

        // On Epic the cost grows with every additional templar:
        // 1s for the first, 1.5s for the second, 2s for the third, and so on.
        Money total = Money.Zero;

        for (int index = 0; index < Templars; index++)
        {
            Money costOfThisTemplar = TemplarUpkeepCostEpicBase + (TemplarUpkeepCostEpicIncrement * index);

            total += costOfThisTemplar;
        }

        return total;
    }
}
