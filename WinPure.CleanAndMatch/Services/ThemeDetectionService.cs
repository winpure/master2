namespace WinPure.CleanAndMatch.Services;

internal class ThemeDetectionService(IWpLogger logger)
{
    private readonly IWpLogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private Control _referenceControl;

    /// <summary>
    /// Sets a reference control to use for theme detection.
    /// If not set, the service will attempt to detect the theme from system colors.
    /// </summary>
    /// <param name="control">The control whose background color will be used for detection.</param>
    public void SetReferenceControl(Control control)
    {
        _referenceControl = control;
    }

    public bool IsDarkTheme()
    {
        try
        {
            if (_referenceControl == null)
            {
                // Fallback to system control background if no reference control is set
                return GetBrightness(SystemColors.Control) < 128;
            }

            return GetBrightness(_referenceControl.BackColor) < 128;
        }
        catch (Exception ex)
        {
            _logger.Error("Error detecting theme. Defaulting to light theme.", ex);
            return false; // Default to light theme on error
        }
    }

    private double GetBrightness(Color color)
    {
        // Standard brightness calculation formula
        // Uses luminance coefficients based on human perception
        var brightness = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
        return brightness;
    }
}
