namespace TaidanaKage.WurmLab.Client.Features.ShardAnalysis;

/// <summary>
/// Reshapes the parsed perimeters into a drawing-friendly form.
/// </summary>
internal static class ProspectingConsolidator
{
    /// <summary>
    /// Groups the leads of every perimeter by direction, so that each resulting slot maps to exactly one drawn region.
    /// </summary>
    /// <param name="perimeters">The parsed perimeters.</param>
    /// <returns>One slot per distance and direction that actually carries leads.</returns>
    internal static List<ProspectingSlot> Consolidate(List<ProspectingPerimeter> perimeters)
    {
        List<ProspectingSlot> slots = [];

        foreach (ProspectingPerimeter perimeter in perimeters)
        {
            // Collect the leads of this perimeter per direction.
            Dictionary<ProspectedDirection, List<ProspectingLead>> leadsByDirection = [];

            foreach (ProspectingLead lead in perimeter.Leads)
            {
                if (leadsByDirection.TryGetValue(lead.Direction, out List<ProspectingLead>? existingLeads) == false)
                {
                    existingLeads = [];
                    leadsByDirection[lead.Direction] = existingLeads;
                }

                existingLeads.Add(lead);
            }

            // Turn every group into a slot.
            foreach (KeyValuePair<ProspectedDirection, List<ProspectingLead>> group in leadsByDirection)
            {
                ProspectingSlot slot = new ProspectingSlot
                {
                    Distance = perimeter.Distance,
                    Direction = group.Key,
                    Leads = group.Value
                };

                slots.Add(slot);
            }
        }

        return slots;
    }
}
