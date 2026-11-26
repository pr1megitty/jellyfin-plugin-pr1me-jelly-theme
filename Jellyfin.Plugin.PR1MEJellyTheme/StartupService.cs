using Jellyfin.Plugin.PR1MEJellyTheme.Helpers;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.PR1MEJellyTheme;

public class StartupService : IScheduledTask
{
    private readonly ILogger<StartupService> _logger;

    public StartupService(ILogger<StartupService> logger)
    {
        _logger = logger;
    }

    public string Name => "PR1ME Jelly Theme startup";

    public string Key => "Jellyfin.Plugin.PR1MEJellyTheme.Startup";

    public string Description => "Registers the PR1ME Jelly Theme stylesheet with File Transformation.";

    public string Category => "Startup Services";

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation("PR1ME Jelly Theme startup: registering the web theme.");

        if (!FileTransformationRegistrar.TryRegister(_logger))
        {
            _logger.LogWarning("PR1ME Jelly Theme is installed but could not attach to the web UI.");
        }

        return Task.CompletedTask;
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo
        {
            Type = TaskTriggerInfoType.StartupTrigger
        };
    }
}
