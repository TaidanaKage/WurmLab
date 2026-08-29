namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

public partial class Page
{
    /// <summary>
    /// A list of phrases that indicate a specific distance in tiles.
    /// Each phrase is, and always will be, unique.
    /// </summary>
    private static readonly (string Phrase, byte Distance)[] TracePhrases =
    [
        ("a trace of", 1),
        ("a slight trace of", 2),
        ("a faint trace of", 3),
        ("a minuscule trace of", 4),
        ("a vague trace of", 5),
        ("an indistinct trace of", 6)
    ];

    private void ProcessInputData()
    {
        if (string.IsNullOrWhiteSpace(_inputText))
        {
            // There's nothing to be parsed.
            _errorMessage = "No data to process.";
            return;
        }

        // Accumulate leads per distance across the whole input.
        Dictionary<byte, List<ProspectingLead>> leadsByDistance = [];

        using StringReader reader = new(_inputText);

        string? line;

        // Let's go through each line of the input text and process it.
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                // Let's skip empty lines, if there are any.
                continue;
            }

            ProcessLine(line, leadsByDistance);
        }

        // Find the farthest distance that actually produced any leads.
        byte maxDistance = 0;
        foreach ((byte distance, List<ProspectingLead> leads) in leadsByDistance)
        {
            if (leads.Count > 0 && distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        Console.WriteLine($"Max distance with leads: {maxDistance}");

        // Build a contiguous range of perimeters, from 1 up to the farthest one with leads.
        // Distances in between that found nothing still get a perimeter, just with no leads.
        _perimeters = [];
        for (byte distance = 1; distance <= maxDistance; distance++)
        {
            if (!leadsByDistance.TryGetValue(distance, out List<ProspectingLead>? leads))
            {
                leads = [];
            }

            _perimeters.Add(new ProspectingPerimeter
            {
                Distance = distance,
                Leads = leads,
            });
        }
    }

    private void ProcessLine(string line, Dictionary<byte, List<ProspectingLead>> leadsByDistance)
    {
        foreach ((string phrase, byte distance) in TracePhrases)
        {
            if (!line.Contains(phrase, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Let's check for a lead (mineral, quality...) in the line.
            ProspectingLead? lead = ParserUtils.CheckForLead(line);
            if (lead is not null)
            {
                if (!leadsByDistance.TryGetValue(distance, out List<ProspectingLead>? leads))
                {
                    leads = [];
                    leadsByDistance[distance] = leads;
                }

                leads.Add(lead);
            }

            // A line describes exactly one trace.
            return;
        }
    }
}
