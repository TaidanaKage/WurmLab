using Microsoft.JSInterop;
using MudBlazor;
using System.Text;

namespace TaidanaKage.WurmLab.Client.Features.ShardAnalyzing;

public partial class Page
{
    private readonly List<BreadcrumbItem> _breadcrumbs =
    [
        new BreadcrumbItem("Home", href: "./"),
            new BreadcrumbItem("Shard Analyzing", href: "ShardAnalyzing")
    ];

    private bool _isProcessed = false;

    /// <summary>
    /// Indicates that processing is in progress, so the Submit button can be disabled.
    /// </summary>
    private bool _isProcessing = false;

    private string? _inputText;

    private List<ProspectingPerimeter> _perimeters = [];

    private string? _svgMarkup;

    private string? _errorMessage;

    /// <summary>
    /// The generated SVG encoded as a data URL, so it can be used as an href of a download link.
    /// </summary>
    private string? _svgDataUrl;

    private async Task SubmitAsync()
    {
        // Disable the Submit button immediately, so the user can't click it multiple times.
        _isProcessing = true;

        _errorMessage = null;
        _perimeters = [];
        _svgMarkup = null;
        _isProcessed = false;

        // Give the renderer a chance to update the UI before the (potentially long) work starts.
        await Task.Yield();

        try
        {
            ProcessInputData();

            if (string.IsNullOrWhiteSpace(_errorMessage))
            {
                // Processing ended without any errors.

                if (_perimeters == null || _perimeters.Count == 0)
                {
                    // Processing didn't return any recognizable prospecting data.
                    _errorMessage = "No prospecting traces were found.";
                }
                else
                {
                    // Data seems to be fine, let's try to generate the SVG markup.
                    _svgMarkup = ImageUtils.GenerateSvgImage(_perimeters);

                    if (_svgMarkup == null)
                    {
                        _errorMessage = "Failed to generate SVG markup.";
                    }
                    else
                    {
                        // Build a data URL for the download link.
                        byte[] svgBytes = Encoding.UTF8.GetBytes(_svgMarkup);
                        string base64 = Convert.ToBase64String(svgBytes);
                        _svgDataUrl = "data:image/svg+xml;base64," + base64;
                    }
                }
            }

            _isProcessed = true;
        }
        finally
        {
            _isProcessing = false;
        }
    }

    private void ResetInput()
    {
        // Reset the current state so the user can input new data.
        _inputText = null;
        _perimeters = [];
        _svgMarkup = null;
        _svgDataUrl = null;
        _errorMessage = null;
        _isProcessed = false;
        _isProcessing = false;
    }
}
