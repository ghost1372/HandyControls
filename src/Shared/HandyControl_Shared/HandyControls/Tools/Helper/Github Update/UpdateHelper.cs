#if NET6_0_OR_GREATER
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using System.Collections.Generic;

namespace HandyControl.Tools;

public static partial class UpdateHelper
{
    private const string GITHUB_API_RELEASES = "https://api.github.com/repos/{0}/{1}/releases";

    /// <summary>
    /// Checks for updates to a specified repository and returns information about the latest stable and pre-release
    /// versions.
    /// </summary>
    /// <param name="username">Identifies the user or organization that owns the repository.</param>
    /// <param name="repository">Specifies the name of the repository to check for updates.</param>
    /// <param name="currentVersion">Represents the current version of the software to compare against available releases.</param>
    /// <returns>Returns a tuple containing information about the latest stable and pre-release updates.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the username or repository is null or empty.</exception>
    public static async Task<(UpdateInfo StableRelease, UpdateInfo PreRelease)> CheckUpdateAsync(string username, string repository, Version currentVersion = null)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        if (string.IsNullOrEmpty(repository))
            throw new ArgumentNullException(nameof(repository));

        var notFoundUpdate = new UpdateInfo { IsExistNewVersion = false };
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", username);

        var url = string.Format(GITHUB_API_RELEASES, username, repository);
        var response = await client.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            // No releases found, return null for both stable and pre-release
            return (notFoundUpdate, notFoundUpdate);
        }

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var releases = JsonSerializer.Deserialize<List<UpdateInfo>>(responseBody, UpdateHelperJsonContext.Default.ListUpdateInfo);
        if (releases == null || releases?.Count == 0)
        {
            return (notFoundUpdate, notFoundUpdate);
        }

        if (currentVersion == null)
        {
            currentVersion = ProcessInfoHelper.GetVersion();
        }

        // Find the latest stable release
        var latestStable = releases
            .Where(r => !r.IsPreRelease)
            .OrderByDescending(r => r.PublishedAt)
            .FirstOrDefault();

        if (latestStable != null)
        {
            var newStableVersionInfo = GetAsVersionInfo(latestStable.TagName);
            var stableMajor = currentVersion.Major == -1 ? 0 : currentVersion.Major;
            var stableMinor = currentVersion.Minor == -1 ? 0 : currentVersion.Minor;
            var stableBuild = currentVersion.Build == -1 ? 0 : currentVersion.Build;
            var stableRevision = currentVersion.Revision == -1 ? 0 : currentVersion.Revision;
            var currentStableVersionInfo = new SystemVersionInfo(stableMajor, stableMinor, stableBuild, stableRevision);

            latestStable.IsExistNewVersion = newStableVersionInfo > currentStableVersionInfo;
        }
        else
        {
            latestStable = new UpdateInfo { IsExistNewVersion = false };
        }

        // Find the latest pre-release
        var latestPreRelease = releases
            .Where(r => r.IsPreRelease)
            .OrderByDescending(r => r.PublishedAt)
            .FirstOrDefault();

        if (latestPreRelease != null)
        {
            var newPreReleaseVersionInfo = GetAsVersionInfo(latestPreRelease.TagName);
            var preReleaseMajor = currentVersion.Major == -1 ? 0 : currentVersion.Major;
            var preReleaseMinor = currentVersion.Minor == -1 ? 0 : currentVersion.Minor;
            var preReleaseBuild = currentVersion.Build == -1 ? 0 : currentVersion.Build;
            var preReleaseRevision = currentVersion.Revision == -1 ? 0 : currentVersion.Revision;
            var currentPreReleaseVersionInfo = new SystemVersionInfo(preReleaseMajor, preReleaseMinor, preReleaseBuild, preReleaseRevision);

            latestPreRelease.IsExistNewVersion = newPreReleaseVersionInfo > currentPreReleaseVersionInfo;
        }
        else
        {
            latestPreRelease = new UpdateInfo { IsExistNewVersion = false };
        }

        return (latestStable, latestPreRelease);
    }

    private static SystemVersionInfo GetAsVersionInfo(string version)
    {
        var nums = GetVersionNumbers(version).Split('.').Select(int.Parse).ToList();

        return nums.Count <= 1
            ? new SystemVersionInfo(nums[0], 0, 0, 0)
            : nums.Count <= 2
            ? new SystemVersionInfo(nums[0], nums[1], 0, 0)
            : nums.Count <= 3
            ? new SystemVersionInfo(nums[0], nums[1], nums[2], 0)
            : new SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
    }

    private static string GetVersionNumbers(string version)
    {
        var allowedChars = "01234567890.";
        return new string(version.Where(c => allowedChars.Contains(c)).ToArray());
    }
}
#endif
