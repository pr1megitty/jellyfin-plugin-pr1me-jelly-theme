using System.Globalization;
using Jellyfin.Plugin.PR1MEJellyTheme.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.PR1MEJellyTheme;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        IServerConfigurationManager serverConfigurationManager) : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        ServerConfigurationManager = serverConfigurationManager;
    }

    public static Plugin? Instance { get; private set; }

    public IServerConfigurationManager ServerConfigurationManager { get; }

    public override string Name => "PR1ME Jelly Theme";

    public override Guid Id => Guid.Parse("84bdf9ee-91e9-4fb7-b97a-f15c1c23ced4");

    public override string Description => "Glass UI theme for Jellyfin: top bar, rows, details page and login page. Style only, never touches playback.";

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = "PR1MEJellyTheme",
                DisplayName = "PR1ME Jelly Theme",
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        };
    }
}
