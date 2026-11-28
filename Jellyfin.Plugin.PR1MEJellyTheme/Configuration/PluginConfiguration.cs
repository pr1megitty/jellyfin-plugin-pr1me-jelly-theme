using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.PR1MEJellyTheme.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration() { }

    // Master switch. When false the plugin serves an empty stylesheet and script.
    public bool Enabled { get; set; } = true;

    // Accent colour as #RRGGBB.
    public string AccentColor { get; set; } = "#C6F432";

    // Glass strength: blur radius in px (0-40) and surface opacity in percent (4-24).
    public int GlassBlur { get; set; } = 24;

    public int GlassOpacity { get; set; } = 8;

    // The soft light at the top of every page. By default it follows the accent colour.
    public bool GlowFollowsAccent { get; set; } = true;

    // Own colour for the top glow (#RRGGBB), used when GlowFollowsAccent is false.
    public string GlowColor { get; set; } = "#6E8BFF";

    // Glow strength in percent (0-30). 0 switches the glow off.
    public int GlowStrength { get; set; } = 14;

    // Corner radius for cards and panels, in px (0-28).
    public int CornerRadius { get; set; } = 16;

    // Per-module toggles.
    public bool ModuleTheme { get; set; } = true;

    public bool ModuleTopBar { get; set; } = true;

    public bool ModuleRows { get; set; } = true;

    public bool ModuleDetails { get; set; } = true;

    public bool ModuleLogin { get; set; } = true;

    // Login page. Empty background means the built-in ambient gradient.
    public string LoginBackgroundUrl { get; set; } = "";

    // 0-90, how much the background is darkened.
    public int LoginDim { get; set; } = 55;
}
