using Svg;
using System.Drawing;

namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

internal static partial class ImageUtils
{
    private static Color GetSlotColour(ProspectingSlot slot)
    {
        // TODO Implement properly.
        return Color.LightGreen;
    }

    [Obsolete]
    private static Color GetMineralColour(ProspectedMineral mineral)
    {
        switch (mineral)
        {
            case ProspectedMineral.Iron:
                return Color.RosyBrown;

            case ProspectedMineral.Copper:
                return Color.SandyBrown;

            case ProspectedMineral.Tin:
                return Color.LightSteelBlue;

            case ProspectedMineral.Zinc:
                return Color.LightBlue;

            case ProspectedMineral.Silver:
                return Color.Silver;

            case ProspectedMineral.Gold:
                return Color.Gold;

            case ProspectedMineral.Lead:
                return Color.SlateGray;

            case ProspectedMineral.Marble:
                return Color.WhiteSmoke;

            case ProspectedMineral.Sandstone:
                return Color.Khaki;

            case ProspectedMineral.Slate:
                return Color.DarkGray;

            case ProspectedMineral.RockSalt:
                return Color.MistyRose;

            case ProspectedMineral.Glimmersteel:
                return Color.PaleTurquoise;

            case ProspectedMineral.Adamantine:
                return Color.MediumPurple;

            case ProspectedMineral.Seryll:
                return Color.PaleGreen;

            case ProspectedMineral.Something:
                return Color.Wheat;

            default:
                return Color.FromArgb(45, 45, 45);
        }
    }

    /// <summary>
    /// Returns a short, manually maintained label for a mineral.
    /// Metals use their periodic table symbols; other materials use short abbreviations.
    /// </summary>
    private static string GetMineralName(ProspectedMineral mineral)
    {
        switch (mineral)
        {
            case ProspectedMineral.Iron:
                return "Iron";

            case ProspectedMineral.Copper:
                return "Copper";

            case ProspectedMineral.Tin:
                return "Tin";

            case ProspectedMineral.Zinc:
                return "Zinc";

            case ProspectedMineral.Silver:
                return "Silver";

            case ProspectedMineral.Gold:
                return "Gold";

            case ProspectedMineral.Lead:
                return "Lead";

            case ProspectedMineral.Marble:
                return "Marble";

            case ProspectedMineral.Sandstone:
                return "Sandstone";

            case ProspectedMineral.Slate:
                return "Slate";

            case ProspectedMineral.RockSalt:
                return "Rock salt";

            case ProspectedMineral.Glimmersteel:
                return "Glimmer";

            case ProspectedMineral.Adamantine:
                return "Ada";

            case ProspectedMineral.Seryll:
                return "Seryll";

            case ProspectedMineral.Something:
                return "Something";

            default:
                // This should never happen.
                return "ERROR";
        }
    }

    /// <summary>
    /// Returns a short, manually maintained label for a quality level.
    /// </summary>
    private static string GetQualityName(ProspectedQuality quality)
    {
        switch (quality)
        {
            case ProspectedQuality.Poor:
                return "20-29";

            case ProspectedQuality.Acceptable:
                return "30-39";

            case ProspectedQuality.Normal:
                return "40-59";

            case ProspectedQuality.Good:
                return "60-79";

            case ProspectedQuality.VeryGood:
                return "80-94";

            case ProspectedQuality.Utmost:
                return "95-99";

            default:
                // This should never happen.
                return "ERROR";
        }
    }

    /// <summary>
    /// Returns a readable text colour (black or white) for the given background colour.
    /// </summary>
    private static Color GetContrastingTextColour(Color background)
    {
        // Relative luminance using the standard sRGB coefficients, in the 0..255 range.
        double luminance = (0.299 * background.R) + (0.587 * background.G) + (0.114 * background.B);

        // Dark backgrounds get white text, light backgrounds get black text.
        if (luminance < 140)
        {
            return Color.White;
        }

        return Color.Black;
    }

    /// <summary>
    /// Returns a readable secondary (dimmed) text colour for the given background colour.
    /// </summary>
    private static Color GetContrastingSecondaryTextColour(Color background)
    {
        Color primary = GetContrastingTextColour(background);

        // Keep the same polarity as the primary colour, only less prominent.
        if (primary == Color.White)
        {
            return Color.Gainsboro;
        }

        return Color.DimGray;
    }

    /// <summary>
    /// Draws the player tile: a plain white background with a stick figure icon centered inside it.
    /// </summary>
    /// <param name="document">The SVG document to draw into.</param>
    /// <param name="pixelX">The X pixel coordinate of the tile's top-left corner.</param>
    /// <param name="pixelY">The Y pixel coordinate of the tile's top-left corner.</param>
    private static void DrawPlayerTile(SvgDocument document, int pixelX, int pixelY)
    {
        // Background of the player tile.
        SvgRectangle originRectangle = new SvgRectangle
        {
            X = pixelX,
            Y = pixelY,
            Width = tileSize,
            Height = tileSize,
            Fill = new SvgColourServer(Color.White),
            Stroke = new SvgColourServer(Color.DarkSlateGray),
            StrokeWidth = 1
        };
        document.Children.Add(originRectangle);

        // Centre of the tile, used as the anchor for the icon.
        int centreX = pixelX + tileSize / 2;
        int centreY = pixelY + tileSize / 2;

        SvgColourServer iconColour = new SvgColourServer(Color.DarkSlateGray);

        // Head.
        SvgCircle head = new SvgCircle
        {
            CenterX = centreX,
            CenterY = centreY - 14,
            Radius = 6,
            Fill = iconColour
        };
        document.Children.Add(head);

        // Body.
        SvgLine body = new SvgLine
        {
            StartX = centreX,
            StartY = centreY - 8,
            EndX = centreX,
            EndY = centreY + 8,
            Stroke = iconColour,
            StrokeWidth = 3
        };
        document.Children.Add(body);

        // Arms.
        SvgLine arms = new SvgLine
        {
            StartX = centreX - 10,
            StartY = centreY - 2,
            EndX = centreX + 10,
            EndY = centreY - 2,
            Stroke = iconColour,
            StrokeWidth = 3
        };
        document.Children.Add(arms);

        // Left leg.
        SvgLine leftLeg = new SvgLine
        {
            StartX = centreX,
            StartY = centreY + 8,
            EndX = centreX - 8,
            EndY = centreY + 20,
            Stroke = iconColour,
            StrokeWidth = 3
        };
        document.Children.Add(leftLeg);

        // Right leg.
        SvgLine rightLeg = new SvgLine
        {
            StartX = centreX,
            StartY = centreY + 8,
            EndX = centreX + 8,
            EndY = centreY + 20,
            Stroke = iconColour,
            StrokeWidth = 3
        };
        document.Children.Add(rightLeg);
    }

    /// <summary>
    /// Draws a subtle 2D grid over the whole image, one line per tile boundary.
    /// Only full-length lines are drawn, so no two lines overlap along their length.
    /// </summary>
    /// <param name="document">The SVG document to draw into.</param>
    /// <param name="columnCount">The number of tile columns in the grid.</param>
    /// <param name="rowCount">The number of tile rows in the grid.</param>
    private static void DrawGrid(SvgDocument document, int columnCount, int rowCount)
    {
        int documentWidth = columnCount * tileSize;
        int documentHeight = rowCount * tileSize;

        // A very light grey keeps the grid readable without competing with the main content.
        SvgColourServer gridColour = new SvgColourServer(Color.Gainsboro);

        // Group the lines so the shared presentation attributes are declared only once.
        SvgGroup gridGroup = new SvgGroup
        {
            Stroke = gridColour,
            StrokeWidth = 1,
            ShapeRendering = SvgShapeRendering.CrispEdges
        };
        document.Children.Add(gridGroup);

        // Vertical lines: one per column boundary, including both outer edges.
        for (int columnIndex = 0; columnIndex <= columnCount; columnIndex++)
        {
            // The half-pixel offset keeps a 1px stroke aligned to the pixel grid instead of straddling it.
            float lineX = columnIndex * tileSize;
            lineX = ClampToDocument(lineX, documentWidth);

            SvgLine verticalLine = new SvgLine
            {
                StartX = lineX,
                StartY = 0,
                EndX = lineX,
                EndY = documentHeight
            };
            gridGroup.Children.Add(verticalLine);
        }

        // Horizontal lines: one per row boundary, including both outer edges.
        for (int rowIndex = 0; rowIndex <= rowCount; rowIndex++)
        {
            float lineY = rowIndex * tileSize;
            lineY = ClampToDocument(lineY, documentHeight);

            SvgLine horizontalLine = new SvgLine
            {
                StartX = 0,
                StartY = lineY,
                EndX = documentWidth,
                EndY = lineY
            };
            gridGroup.Children.Add(horizontalLine);
        }
    }

    /// <summary>
    /// Shifts a grid line coordinate by half a pixel so a 1px stroke lands on a whole pixel,
    /// pulling the outermost lines inwards so they are not clipped by the document edge.
    /// </summary>
    private static float ClampToDocument(float coordinate, int documentSize)
    {
        if (coordinate <= 0)
        {
            // The first line sits just inside the top/left edge.
            return 0.5f;
        }

        if (coordinate >= documentSize)
        {
            // The last line sits just inside the bottom/right edge.
            return documentSize - 0.5f;
        }

        return coordinate + 0.5f;
    }
}
