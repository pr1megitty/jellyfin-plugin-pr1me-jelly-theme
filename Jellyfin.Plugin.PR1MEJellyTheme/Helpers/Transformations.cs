using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MediaBrowser.Common.Net;

namespace Jellyfin.Plugin.PR1MEJellyTheme.Helpers;

public static partial class Transformations
{
    // Tiny inline guard that lives in <head>. It lets you turn the theme off from the address bar
    // (add ?pr1me=off to the URL, and ?pr1me=on to bring it back) even if a style ever breaks the page.
    private const string KillSwitch =
        "<script data-pr1me=\"true\">(function(){try{var l=document.getElementById('pr1me-theme-css');var u=location.href;" +
        "if(/[?&#]pr1me=off/.test(u)){localStorage.setItem('pr1me-off','1');}" +
        "else if(/[?&#]pr1me=on/.test(u)){localStorage.removeItem('pr1me-off');}" +
        "if(localStorage.getItem('pr1me-off')==='1'){if(l){l.remove();}document.documentElement.setAttribute('data-pr1me','off');}" +
        "}catch(e){}})();</script>";

    [GeneratedRegex("<link\\b[^>]*data-pr1me=\"true\"[^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex ExistingLinkRegex();

    [GeneratedRegex("<script\\b[^>]*data-pr1me=\"true\"[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ExistingScriptRegex();

    public static string IndexTransformation(PatchRequestPayload payload)
    {
        string contents = payload.Contents ?? string.Empty;
        string basePath = GetBasePath();

        contents = ExistingLinkRegex().Replace(contents, string.Empty);
        contents = ExistingScriptRegex().Replace(contents, string.Empty);

        string headMarkup =
            $"<link id=\"pr1me-theme-css\" data-pr1me=\"true\" rel=\"stylesheet\" href=\"{basePath}/pr1mejellytheme/theme.css\">" +
            KillSwitch;
        string bodyMarkup =
            $"<script data-pr1me=\"true\" defer src=\"{basePath}/pr1mejellytheme/theme.js\"></script>";

        int headClosing = contents.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        if (headClosing >= 0)
        {
            contents = contents.Insert(headClosing, headMarkup);
        }

        int bodyClosing = contents.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
        if (bodyClosing >= 0)
        {
            contents = contents.Insert(bodyClosing, bodyMarkup);
        }

        return contents;
    }

    private static string GetBasePath()
    {
        try
        {
            NetworkConfiguration networkConfiguration = Plugin.Instance!.ServerConfigurationManager.GetNetworkConfiguration();
            if (!string.IsNullOrWhiteSpace(networkConfiguration.BaseUrl))
            {
                return $"/{networkConfiguration.BaseUrl.Trim().Trim('/')}";
            }
        }
        catch (Exception)
        {
            // Fall through to the root path.
        }

        return string.Empty;
    }
}

public class PatchRequestPayload
{
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}
