namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

/// <summary>
/// A collection of helper methods to parse the event log lines and extract relevant information.
/// </summary>
internal static class ParserUtils
{
    /// <summary>
    /// Distance-agnostic check if the line contains a mineral, quality, flint, salt, etc.
    /// </summary>
    internal static ProspectingLead? CheckForLead(string line)
    {
        // Direction
        ProspectedDirection? direction = ParseDirection(line);
        if (!direction.HasValue)
        {
            // Each lead must have a direction, so if we can't find one, it's not a lead.
            return null;
        }

        // Flint
        bool hasFlint = HasFlint(line);
        if (hasFlint)
        {
            // If the line mentions flint, we can be sure it doesn't contain anything else.
            return new ProspectingLead
            {
                HasFlint = true,
                Direction = direction.Value,
                Mineral = null,
                Quality = null,
                HasSalt = false
            };
        }

        // Salt
        bool hasSalt = HasSalt(line);
        if (hasSalt)
        {
            // If the line mentions salt, we can be sure it doesn't contain anything else.
            return new ProspectingLead
            {
                HasSalt = true,
                Direction = direction.Value,
                Mineral = null,
                Quality = null,
                HasFlint = false
            };
        }

        // Mineral and quality
        ProspectedMineral? mineral = ParseMineral(line);
        ProspectedQuality? quality = ParseQuality(line);

        if (!mineral.HasValue && !quality.HasValue)
        {
            // If the line doesn't contain a mineral or quality, it's not a lead.
            return null;
        }

        // We found a lead.
        return new ProspectingLead
        {
            Direction = direction.Value,
            Mineral = mineral,
            Quality = quality,
            HasFlint = false,
            HasSalt = false
        };
    }

    private static ProspectedDirection? ParseDirection(string line)
    {
        // 8 secondary intercardinal directions
        if (line.Contains("(east of north)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.EastOfNorth;
        }
        else if (line.Contains("(west of north)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.WestOfNorth;
        }
        else if (line.Contains("(north of east)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.NorthOfEast;
        }
        else if (line.Contains("(south of east)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.SouthOfEast;
        }
        else if (line.Contains("(east of south)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.EastOfSouth;
        }
        else if (line.Contains("(west of south)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.WestOfSouth;
        }
        else if (line.Contains("(north of west)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.NorthOfWest;
        }
        else if (line.Contains("(south of west)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.SouthOfWest;
        }
        // 4 diagonal (ordinal) directions
        else if (line.Contains("(northeast)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.NorthEast;
        }
        else if (line.Contains("(southeast)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.SouthEast;
        }
        else if (line.Contains("(southwest)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.SouthWest;
        }
        else if (line.Contains("(northwest)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.NorthWest;
        }
        // 4 cardinal directions
        else if (line.Contains("(north)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.North;
        }
        else if (line.Contains("(south)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.South;
        }
        else if (line.Contains("(east)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.East;
        }
        else if (line.Contains("(west)", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedDirection.West;
        }
        return null;
    }

    private static bool HasFlint(string line)
    {
        return line.Contains(" flint ", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasSalt(string line)
    {
        if (line.Contains(" rock salt ", StringComparison.OrdinalIgnoreCase))
        {
            // Rock salt is a mineral, not the same as salt detection.
            return false;
        }
        return line.Contains(" salt ", StringComparison.OrdinalIgnoreCase);
    }

    private static ProspectedQuality? ParseQuality(string line)
    {
        if (line.Contains("poor quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.Poor;
        }
        else if (line.Contains("acceptable quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.Acceptable;
        }
        else if (line.Contains("normal quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.Normal;
        }
        // "very good quality" must be checked before "good quality", because it contains "good quality" as a substring.
        else if (line.Contains("very good quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.VeryGood;
        }
        else if (line.Contains("good quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.Good;
        }
        else if (line.Contains("utmost quality", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedQuality.Utmost;
        }
        return null;
    }

    private static ProspectedMineral? ParseMineral(string line)
    {
        // The prospecting skill is too low to identify the mineral.
        if (line.Contains(" something, but cannot quite make it out ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Something;
        }
        else if (line.Contains(" adamantine ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Adamantine;
        }
        else if (line.Contains(" rock salt ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.RockSalt;
        }
        else if (line.Contains(" glimmersteel ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Glimmersteel;
        }
        else if (line.Contains(" seryll ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Seryll;
        }
        else if (line.Contains(" marble ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Marble;
        }
        else if (line.Contains(" sandstone ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Sandstone;
        }
        else if (line.Contains(" slate ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Slate;
        }
        else if (line.Contains(" copper ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Copper;
        }
        else if (line.Contains(" tin ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Tin;
        }
        else if (line.Contains(" iron ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Iron;
        }
        else if (line.Contains(" silver ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Silver;
        }
        else if (line.Contains(" gold ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Gold;
        }
        else if (line.Contains(" lead ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Lead;
        }
        else if (line.Contains(" zinc ", StringComparison.OrdinalIgnoreCase))
        {
            return ProspectedMineral.Zinc;
        }
        return null;
    }
}
