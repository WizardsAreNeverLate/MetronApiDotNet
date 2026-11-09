using System;
using System.IO;

namespace Metron.Api.Sample;

internal static class TokenSource
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
