using System;
using System.IO;
using System.Threading.Tasks;
using TUnit.Core;

namespace Metron.Api.Tests;

internal static class LiveToken
{
    public static string? Find()
    {
        var fromEnv = Environment.GetEnvironmentVariable("METRON_TOKEN");
        if (!string.IsNullOrWhiteSpace(fromEnv)) return fromEnv.Trim();

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "secrets", "tokens.txt");
            if (File.Exists(candidate))
            {
                var text = File.ReadAllText(candidate).Trim();
                return text.Length == 0 ? null : text;
            }
        }
        return null;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
internal sealed class RequiresTokenAttribute : SkipAttribute
{
    public RequiresTokenAttribute() : base("No Metron token (METRON_TOKEN or secrets/tokens.txt)") { }

    public override Task<bool> ShouldSkip(TestRegisteredContext context) =>
        Task.FromResult(LiveToken.Find() is null);
}
