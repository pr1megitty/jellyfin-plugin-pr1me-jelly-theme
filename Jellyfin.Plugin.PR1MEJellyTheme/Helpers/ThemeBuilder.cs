using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.PR1MEJellyTheme.Configuration;

namespace Jellyfin.Plugin.PR1MEJellyTheme.Helpers;

/// <summary>
/// Builds the stylesheet from the embedded module files plus the admin's settings.
/// Only modules that are switched on are included, so a disabled module costs nothing.
/// </summary>
public static partial class ThemeBuilder
{
    private const string DefaultAccent = "#C6F432";

    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex HexColorRegex();

    public static string ReadResource(string fileName)
    {
        Assembly assembly = typeof(ThemeBuilder).Assembly;
        string name = $"{typeof(Plugin).Namespace}.Web.{fileName}";
        using Stream? stream = assembly.GetManifestResourceStream(name);
        if (stream is null)
        {
            return string.Empty;
        }

        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static string BuildCss(PluginConfiguration config)
    {
        if (!config.Enabled)
        {
            return "/* PR1ME Jelly Theme is switched off in the plugin settings. */\n";
        }

        StringBuilder css = new StringBuilder();
        css.AppendLine("/* PR1ME Jelly Theme */");
        css.AppendLine(BuildVariables(config));

        if (config.ModuleTheme)
        {
            css.AppendLine(ReadResource("base.css"));
        }

        if (config.ModuleTopBar)
        {
            css.AppendLine(ReadResource("topbar.css"));
        }

        if (config.ModuleRows)
        {
            css.AppendLine(ReadResource("rows.css"));
        }

        if (config.ModuleDetails)
        {
            css.AppendLine(ReadResource("details.css"));
        }

        if (config.ModuleLogin)
        {
            css.AppendLine(BuildLoginVariables(config));
            css.AppendLine(ReadResource("login.css"));
        }

        return css.ToString();
    }

    public static string BuildScript(PluginConfiguration config)
    {
        if (!config.Enabled)
        {
            return "/* PR1ME Jelly Theme is switched off in the plugin settings. */\n";
        }

        return ReadResource("theme.js");
    }

    public static string NormalizeAccent(string? value)
    {
        string candidate = (value ?? string.Empty).Trim();
        return HexColorRegex().IsMatch(candidate) ? candidate.ToUpperInvariant() : DefaultAccent;
    }

    private static string BuildVariables(PluginConfiguration config)
    {
        string accent = NormalizeAccent(config.AccentColor);
        int r = int.Parse(accent.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        int g = int.Parse(accent.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        int b = int.Parse(accent.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        // Pick dark or light text for use on top of the accent colour (WCAG relative luminance).
        double luminance = (0.2126 * Linear(r)) + (0.7152 * Linear(g)) + (0.0722 * Linear(b));
        string onAccent = luminance > 0.38 ? "#0B0B0C" : "#FFFFFF";

        int blur = Math.Clamp(config.GlassBlur, 0, 40);
        int opacity = Math.Clamp(config.GlassOpacity, 4, 60);
        int radius = Math.Clamp(config.CornerRadius, 0, 28);
        string alpha = (opacity / 100.0).ToString("0.###", CultureInfo.InvariantCulture);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(":root {");
        sb.AppendLine($"  --pr1me-accent: {accent};");
        sb.AppendLine($"  --pr1me-accent-rgb: {r}, {g}, {b};");
        sb.AppendLine($"  --pr1me-accent-ch: {r} {g} {b};");
        sb.AppendLine($"  --pr1me-on-accent: {onAccent};");
        sb.AppendLine($"  --pr1me-blur: {blur}px;");
        sb.AppendLine($"  --pr1me-glass-alpha: {alpha};");
        sb.AppendLine($"  --pr1me-radius: {radius}px;");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildLoginVariables(PluginConfiguration config)
    {
        int dim = Math.Clamp(config.LoginDim, 0, 90);
        string dimValue = (dim / 100.0).ToString("0.##", CultureInfo.InvariantCulture);
        string url = SanitizeUrl(config.LoginBackgroundUrl);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(":root {");
        sb.AppendLine($"  --pr1me-login-dim: {dimValue};");
        sb.AppendLine(url.Length > 0
            ? $"  --pr1me-login-image: url(\"{url}\");"
            : "  --pr1me-login-image: none;");
        sb.AppendLine("}");
        return sb.ToString();
    }

    // Only plain http(s) or site-relative URLs, with anything that could break out of url("...") rejected.
    private static string SanitizeUrl(string? value)
    {
        string candidate = (value ?? string.Empty).Trim();
        if (candidate.Length == 0 || candidate.Length > 2048)
        {
            return string.Empty;
        }

        bool allowed = candidate.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || candidate.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || candidate.StartsWith('/');
        if (!allowed)
        {
            return string.Empty;
        }

        foreach (char c in candidate)
        {
            if (c == '"' || c == '\'' || c == '\\' || c == '(' || c == ')' || c == '<' || c == '>' || char.IsControl(c) || char.IsWhiteSpace(c))
            {
                return string.Empty;
            }
        }

        return candidate;
    }

    private static double Linear(int channel)
    {
        double c = channel / 255.0;
        return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }
}
