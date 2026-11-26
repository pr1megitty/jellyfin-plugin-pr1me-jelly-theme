using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.PR1MEJellyTheme.Helpers;

public static class FileTransformationRegistrar
{
    private const string PluginInterfaceTypeName = "Jellyfin.Plugin.FileTransformation.PluginInterface";
    private const string TransformationId = "1f1e927a-e3c4-4396-a2ef-ddc8b519095a";

    public static bool TryRegister(ILogger logger)
    {
        try
        {
            Assembly? assembly = AssemblyLoadContext.All
                .SelectMany(context => context.Assemblies)
                .FirstOrDefault(candidate =>
                    candidate.FullName?.Contains(".FileTransformation", StringComparison.OrdinalIgnoreCase) ?? false);

            if (assembly is null)
            {
                logger.LogWarning("PR1ME Jelly Theme needs the File Transformation plugin, but it was not found. Install it and restart Jellyfin.");
                return false;
            }

            Type? pluginInterfaceType = assembly.GetType(PluginInterfaceTypeName);
            MethodInfo? registerMethod = pluginInterfaceType?.GetMethod("RegisterTransformation");
            if (registerMethod is null)
            {
                logger.LogWarning("File Transformation registration interface was not found.");
                return false;
            }

            JObject payload = new JObject
            {
                { "id", TransformationId },
                { "fileNamePattern", "index.html" },
                { "callbackAssembly", typeof(FileTransformationRegistrar).Assembly.FullName },
                { "callbackClass", typeof(Transformations).FullName },
                { "callbackMethod", nameof(Transformations.IndexTransformation) }
            };

            object? result = registerMethod.Invoke(null, new object?[] { payload });
            if (result is false)
            {
                logger.LogWarning("File Transformation rejected the PR1ME Jelly Theme registration.");
                return false;
            }

            logger.LogInformation("PR1ME Jelly Theme registered with File Transformation.");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register PR1ME Jelly Theme with File Transformation.");
            return false;
        }
    }
}
