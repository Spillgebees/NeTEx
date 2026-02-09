using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Spillgebees.NeTEx.Generator.Configuration;

namespace Spillgebees.NeTEx.Generator.Services;

public static partial class SchemaDownloader
{
    private static readonly HttpClient HttpClient = CreateHttpClient();

    /// <summary>
    /// Downloads and extracts NeTEx schemas from the official NeTEx-CEN GitHub repository.
    /// The caller is responsible for cleaning up the returned <see cref="SchemaDirectory"/>.
    /// </summary>
    public static async Task<SchemaDirectory> DownloadAndExtractAsync(
        string versionOrRef,
        bool isTag,
        Action<string>? log = null,
        CancellationToken cancellationToken = default)
    {
        var url = isTag
            ? string.Format(CultureInfo.InvariantCulture, GeneratorDefaults.GitHubArchiveUrlTemplate, versionOrRef)
            : string.Format(CultureInfo.InvariantCulture, GeneratorDefaults.GitHubArchiveRefUrlTemplate, versionOrRef);

        log?.Invoke($"Downloading schemas from {url}...");

        var tempDir = Path.Combine(Path.GetTempPath(), "netex-schemas", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            using var response = await HttpClient.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException(
                    $"Version or ref '{versionOrRef}' was not found in the NeTEx repository. "
                    + "Use 'list-versions' to see available versions.");
            }

            response.EnsureSuccessStatusCode();

            var archivePath = Path.Combine(tempDir, "netex.tar.gz");
            await using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var fileStream = File.Create(archivePath))
            {
                await responseStream.CopyToAsync(fileStream, cancellationToken);
            }

            log?.Invoke("Extracting schemas...");

            var extractDir = Path.Combine(tempDir, "extracted");
            Directory.CreateDirectory(extractDir);

            await using (var archiveStream = File.OpenRead(archivePath))
            await using (var gzipStream = new GZipStream(archiveStream, CompressionMode.Decompress))
            {
                await TarFile.ExtractToDirectoryAsync(gzipStream, extractDir, overwriteFiles: true, cancellationToken);
            }

            // GitHub archives extract to a single folder like NeTEx-{version}/
            var extractedDirs = Directory.GetDirectories(extractDir);
            if (extractedDirs.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Expected exactly one top-level directory in the archive, but found {extractedDirs.Length}.");
            }

            var xsdDir = Path.Combine(extractedDirs[0], "xsd");
            if (!Directory.Exists(xsdDir))
            {
                throw new InvalidOperationException($"XSD directory not found at {xsdDir}");
            }

            log?.Invoke($"Schemas extracted to {xsdDir}");
            return new SchemaDirectory(xsdDir, tempDir);
        }
        catch
        {
            // Clean up on failure so we don't leak temp directories
            TryDeleteDirectory(tempDir);
            throw;
        }
    }

    public static async Task<IReadOnlyList<string>> ListVersionsAsync(
        CancellationToken cancellationToken = default)
    {
        var versions = new List<string>();
        var url = (string?)$"{GeneratorDefaults.GitHubApiTagsUrl}?per_page=100";

        while (url is not null)
        {
            using var response = await HttpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Failed to fetch versions from GitHub (HTTP {(int)response.StatusCode}). "
                    + "Please check your network connection and try again.");
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var tags = JsonDocument.Parse(json);

            foreach (var tag in tags.RootElement.EnumerateArray())
            {
                if (tag.TryGetProperty("name", out var name) && name.GetString() is { } tagName)
                {
                    versions.Add(tagName);
                }
            }

            // Check for pagination via Link header
            url = null;
            if (response.Headers.TryGetValues("Link", out var linkHeaders))
            {
                var linkHeader = string.Join(",", linkHeaders);
                var nextMatch = NextLinkPattern().Match(linkHeader);
                if (nextMatch.Success)
                {
                    url = nextMatch.Groups[1].Value;
                }
            }
        }

        return versions;
    }

    internal static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup; don't mask the original exception
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        var client = new HttpClient(handler);

        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "0.0.0";

        client.DefaultRequestHeaders.UserAgent.ParseAdd($"Spillgebees.NeTEx.Generator/{version}");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

        return client;
    }

    [GeneratedRegex(@"<([^>]+)>;\s*rel=""next""")]
    private static partial Regex NextLinkPattern();
}

/// <summary>
/// Represents an extracted schema directory with its parent temp directory.
/// Disposing this instance cleans up the temporary files.
/// </summary>
public sealed class SchemaDirectory : IDisposable
{
    public SchemaDirectory(string xsdPath, string tempRootPath)
    {
        XsdPath = xsdPath;
        TempRootPath = tempRootPath;
    }

    /// <summary>
    /// Path to the xsd/ directory containing the NeTEx schemas.
    /// </summary>
    public string XsdPath { get; }

    /// <summary>
    /// Root of the temporary directory tree. Deleted on <see cref="Dispose"/>.
    /// </summary>
    public string TempRootPath { get; }

    public void Dispose() => SchemaDownloader.TryDeleteDirectory(TempRootPath);
}
