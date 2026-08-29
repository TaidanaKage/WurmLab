using Svg;
using System.Drawing;

namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

/// <summary>
/// A collection of helper methods to generate images from the parsed data.
/// </summary>
internal static partial class ImageUtils
{
    /// <summary>
    /// Hardcoded size of each tile (square) in pixels.
    /// </summary>
    private const int tileSize = 75;

    /// <summary>
    /// Hardcoded font size in pixels.
    /// </summary>
    private const int fontSize = 12;

    /// <summary>
    /// Generates an SVG image representing the 2D tile-based grid around the player's position.
    /// </summary>
    /// <param name="perimeters">
    /// The list of prospecting perimeters (rings around player) to visualize.
    /// The list is ordered by distance, with the closest perimeter first.
    /// </param>
    /// <returns>A string containing the SVG image, or null if the image could not be generated.</returns>
    internal static string? GenerateSvgImage(List<ProspectingPerimeter> perimeters)
    {
        if (perimeters == null || perimeters.Count == 0)
        {
            // Invalid input: no perimeters provided.
            return null;
        }

        // Determine the grid dimensions: the player tile plus one ring of tiles per perimeter on each side.
        int columnCount = 2 * perimeters.Count + 1;
        int rowCount = columnCount;

        int documentWidth = columnCount * tileSize;
        int documentHeight = rowCount * tileSize;

        // Create the root SVG document sized to fit the whole grid.
        SvgDocument document = new SvgDocument
        {
            Width = documentWidth,
            Height = documentHeight,
            ViewBox = new SvgViewBox(0, 0, documentWidth, documentHeight)
        };

        // Add a background rectangle covering the entire grid.
        SvgRectangle background = new SvgRectangle
        {
            X = 0,
            Y = 0,
            Width = documentWidth,
            Height = documentHeight,
            Fill = new SvgColourServer(Color.WhiteSmoke)
        };
        document.Children.Add(background);

        // Draw the tile grid before any content, so the opaque regions painted later cover it.
        DrawGrid(document, columnCount, rowCount);

        // The player is always located in the middle tile of the grid.
        int playerColumnIndex = columnCount / 2;
        int playerRowIndex = rowCount / 2;

        int playerPixelX = playerColumnIndex * tileSize;
        int playerPixelY = playerRowIndex * tileSize;

        DrawPlayerTile(document, playerPixelX, playerPixelY);


        // Consolidate the leads so that each direction is drawn exactly once.
        List<ProspectingSlot> slots = ProspectingConsolidator.Consolidate(perimeters);


        DrawSlots(document, slots, perimeters.Count);


        // Serialize the document to an SVG string.
        string svgMarkup = document.GetXML();

        return svgMarkup;
    }

    /// <summary>
    /// Draws every consolidated slot as a single region, with its labels stacked inside.
    /// </summary>
    /// <param name="document">The SVG document to draw into.</param>
    /// <param name="slots">The consolidated slots.</param>
    /// <param name="maxDistance">The largest perimeter distance, used to place the origin of the coordinate system.</param>
    private static void DrawSlots(SvgDocument document, List<ProspectingSlot> slots, int maxDistance)
    {
        foreach (ProspectingSlot slot in slots)
        {
            TileRegion? region = DirectionGeometry.Resolve(slot.Direction, slot.Distance);
            if (region is null)
            {
                // This direction covers no tiles at this distance.
                continue;
            }

            DrawSlot(document, slot, region.Value, maxDistance);
        }
    }

    /// <summary>
    /// Draws a single slot: one bordered rectangle covering the whole region, plus the stacked labels.
    /// No inner tile borders are drawn, because the exact tile within the region is unknown.
    /// </summary>
    private static void DrawSlot(SvgDocument document, ProspectingSlot slot, TileRegion region, int maxDistance)
    {
        // Convert the tile region into pixel space.
        // X grows towards East (right), Y grows towards North (up on screen means a smaller pixel Y).
        int pixelX = (region.MinX + maxDistance) * tileSize;
        int pixelY = (maxDistance - region.MaxY) * tileSize;

        int pixelWidth = region.WidthInTiles * tileSize;
        int pixelHeight = region.HeightInTiles * tileSize;

        // Pick the fill colour from the leads of this slot.
        Color fillColour = GetSlotColour(slot);
        Color textColour = GetContrastingTextColour(fillColour);

        SvgRectangle regionRectangle = new SvgRectangle
        {
            X = pixelX,
            Y = pixelY,
            Width = pixelWidth,
            Height = pixelHeight,
            Fill = new SvgColourServer(fillColour),
            Stroke = new SvgColourServer(Color.DarkSlateGray),
            StrokeWidth = 1
        };
        document.Children.Add(regionRectangle);

        // Build the label lines for all leads of this slot.
        List<string> labels = BuildLabels(slot);

        // Centre of the region, used as the anchor for the label block.
        int centreX = pixelX + pixelWidth / 2;
        int centreY = pixelY + pixelHeight / 2;

        DrawLabels(document, labels, centreX, centreY, region.IsVertical, textColour);
    }

    /// <summary>
    /// Draws a block of centered label lines, rotating them by 90 degrees for vertical regions.
    /// </summary>
    private static void DrawLabels(SvgDocument document, List<string> labels, int centreX, int centreY, bool isVertical, Color textColour)
    {
        if (labels.Count == 0)
        {
            return;
        }

        const int lineHeight = fontSize + 2;

        // Vertically centre the whole block of lines around the region centre.
        int blockHeight = labels.Count * lineHeight;
        int firstLineOffset = (lineHeight - blockHeight) / 2;

        // A rotated region needs a single group carrying the rotation, so the lines stay stacked correctly.
        SvgGroup labelGroup = new SvgGroup();

        if (isVertical)
        {
            // Rotate around the centre of the region: the lines then read bottom-to-top along the run.
            labelGroup.Transforms = [new Svg.Transforms.SvgRotate(-90, centreX, centreY)];
        }

        document.Children.Add(labelGroup);

        for (int lineIndex = 0; lineIndex < labels.Count; lineIndex++)
        {
            int lineY = centreY + firstLineOffset + lineIndex * lineHeight;

            SvgText label = new SvgText(labels[lineIndex])
            {
                X = [centreX],
                Y = [lineY],
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = fontSize,
                FontFamily = "sans-serif",
                Fill = new SvgColourServer(textColour)
            };

            labelGroup.Children.Add(label);
        }
    }

    /// <summary>
    /// Builds the label lines for a slot, in a stable order, so that repeated renders look identical.
    /// </summary>
    private static List<string> BuildLabels(ProspectingSlot slot)
    {
        List<string> labels = [];

        bool hasFlint = false;
        bool hasSalt = false;

        // Count the minerals up front: a slot holding several minerals is rendered more compactly.
        int mineralCount = 0;

        foreach (ProspectingLead lead in slot.Leads)
        {
            if (lead.Mineral is not null)
            {
                mineralCount++;
            }
        }

        bool combineNameAndQuality = mineralCount > 1;

        foreach (ProspectingLead lead in slot.Leads)
        {
            if (lead.Mineral is not null)
            {
                string mineralName = GetMineralName(lead.Mineral.Value);

                if (lead.Quality is null)
                {
                    // Nothing to combine: only the mineral name is known.
                    labels.Add(mineralName);
                }
                else
                {
                    string qualityName = GetQualityName(lead.Quality.Value);

                    if (combineNameAndQuality)
                    {
                        // Several minerals share this slot: keep each one on a single line.
                        labels.Add($"{mineralName} {qualityName}");
                    }
                    else
                    {
                        // A single mineral: keep the existing two-line layout.
                        labels.Add(mineralName);
                        labels.Add(qualityName);
                    }
                }
            }

            // Flint and salt are independent of the mineral, and are collected across all leads of the slot.
            if (lead.HasFlint)
            {
                hasFlint = true;
            }

            if (lead.HasSalt)
            {
                hasSalt = true;
            }
        }

        if (hasFlint)
        {
            labels.Add("Flint");
        }

        if (hasSalt)
        {
            labels.Add("Salt");
        }

        return labels;
    }
}
