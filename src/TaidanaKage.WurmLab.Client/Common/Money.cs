using System.Globalization;
using System.Text;

namespace TaidanaKage.WurmLab.Client.Common;

/// <summary>
/// Represents an amount of money in Wurm Unlimited.
/// </summary>
/// <remarks>
/// The value is stored internally as a single number - the total amount of iron coins.
/// The coin denominations are:
/// 100 iron = 1 copper, 100 copper = 1 silver, 100 silver = 1 gold.
/// </remarks>
internal readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    /// <summary>
    /// The number of iron coins in a single copper coin.
    /// </summary>
    internal const long IronPerCopper = 100;

    /// <summary>
    /// The number of iron coins in a single silver coin.
    /// </summary>
    internal const long IronPerSilver = IronPerCopper * 100;

    /// <summary>
    /// The number of iron coins in a single gold coin.
    /// </summary>
    internal const long IronPerGold = IronPerSilver * 100;

    /// <summary>
    /// A zero amount of money.
    /// </summary>
    internal static readonly Money Zero = new(0);

    /// <summary>
    /// The total value of this amount, expressed in iron coins.
    /// </summary>
    internal long TotalIron { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> struct.
    /// </summary>
    /// <param name="totalIron">The total value, in iron coins.</param>
    internal Money(long totalIron)
    {
        TotalIron = totalIron;
    }

    /// <summary>
    /// Creates a new instance from the individual coin denominations.
    /// </summary>
    /// <param name="gold">The number of gold coins.</param>
    /// <param name="silver">The number of silver coins.</param>
    /// <param name="copper">The number of copper coins.</param>
    /// <param name="iron">The number of iron coins.</param>
    /// <returns>The combined amount of money.</returns>
    internal static Money FromCoins(long gold = 0, long silver = 0, long copper = 0, long iron = 0)
    {
        // Convert every denomination into iron and sum it up.
        long total = gold * IronPerGold;
        total += silver * IronPerSilver;
        total += copper * IronPerCopper;
        total += iron;

        return new Money(total);
    }

    /// <summary>
    /// Gets the whole gold coins contained in this amount.
    /// </summary>
    internal long Gold
    {
        get
        {
            long absolute = Math.Abs(TotalIron);

            return absolute / IronPerGold;
        }
    }

    /// <summary>
    /// Gets the whole silver coins remaining after the gold coins have been taken out.
    /// </summary>
    internal long Silver
    {
        get
        {
            long absolute = Math.Abs(TotalIron);

            return absolute % IronPerGold / IronPerSilver;
        }
    }

    /// <summary>
    /// Gets the whole copper coins remaining after the silver coins have been taken out.
    /// </summary>
    internal long Copper
    {
        get
        {
            long absolute = Math.Abs(TotalIron);

            return absolute % IronPerSilver / IronPerCopper;
        }
    }

    /// <summary>
    /// Gets the iron coins remaining after the copper coins have been taken out.
    /// </summary>
    internal long Iron
    {
        get
        {
            long absolute = Math.Abs(TotalIron);

            return absolute % IronPerCopper;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this amount is negative.
    /// </summary>
    internal bool IsNegative
    {
        get
        {
            return TotalIron < 0;
        }
    }

    /// <summary>
    /// Formats the amount for display, for example "1g 23s 45c 67i".
    /// Denominations with a zero value are omitted, unless the whole amount is zero.
    /// </summary>
    /// <returns>The formatted amount.</returns>
    public override string ToString()
    {
        // A zero amount has no denominations to show, so use a simple fixed representation.
        if (TotalIron == 0)
        {
            return "0i";
        }

        var builder = new StringBuilder();

        // The sign is rendered once, in front of the whole amount.
        if (IsNegative)
        {
            builder.Append('-');
        }

        AppendCoin(builder, Gold, 'g');
        AppendCoin(builder, Silver, 's');
        AppendCoin(builder, Copper, 'c');
        AppendCoin(builder, Iron, 'i');

        return builder.ToString();
    }

    /// <summary>
    /// Appends a single denomination to the given builder, if its value is not zero.
    /// </summary>
    /// <param name="builder">The builder to append to.</param>
    /// <param name="value">The number of coins of this denomination.</param>
    /// <param name="suffix">The single letter suffix of this denomination.</param>
    private static void AppendCoin(StringBuilder builder, long value, char suffix)
    {
        if (value == 0)
        {
            return;
        }

        // Separate the denominations with a space, but only between them.
        if (builder.Length > 0 && builder[^1] != '-')
        {
            builder.Append(' ');
        }

        builder.Append(value.ToString(CultureInfo.InvariantCulture));
        builder.Append(suffix);
    }

    public static Money operator +(Money left, Money right)
    {
        return new Money(left.TotalIron + right.TotalIron);
    }

    public static Money operator -(Money left, Money right)
    {
        return new Money(left.TotalIron - right.TotalIron);
    }

    public static Money operator -(Money value)
    {
        return new Money(-value.TotalIron);
    }

    public static Money operator *(Money left, long multiplier)
    {
        return new Money(left.TotalIron * multiplier);
    }

    public static Money operator *(long multiplier, Money right)
    {
        return new Money(right.TotalIron * multiplier);
    }

    public static bool operator ==(Money left, Money right)
    {
        return left.TotalIron == right.TotalIron;
    }

    public static bool operator !=(Money left, Money right)
    {
        return left.TotalIron != right.TotalIron;
    }

    public static bool operator <(Money left, Money right)
    {
        return left.TotalIron < right.TotalIron;
    }

    public static bool operator >(Money left, Money right)
    {
        return left.TotalIron > right.TotalIron;
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.TotalIron <= right.TotalIron;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.TotalIron >= right.TotalIron;
    }

    public bool Equals(Money other)
    {
        return TotalIron == other.TotalIron;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
        {
            return Equals(other);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return TotalIron.GetHashCode();
    }

    public int CompareTo(Money other)
    {
        return TotalIron.CompareTo(other.TotalIron);
    }
}
