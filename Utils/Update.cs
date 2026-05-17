using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace xsoverlay_tweak.Utils
{
    internal class Update
    {
        private const string repo = "chaixshot/xsoverlay-tweak";
        private const string GitHubRepoUrl = $"https://github.com/{repo}";
        private const string GitHubReleasesUrl = $"https://github.com/{repo}/releases";
        private const string GitHubLatestReleaseApi = $"https://api.github.com/repos/{repo}/releases/latest";

        private static async Task<string> GetLatestVersionAsync()
        {
            using var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("xsoverlay-tweak");
            var response = await client.GetStringAsync(GitHubLatestReleaseApi);

            var m = Regex.Match(response, "\"tag_name\"\\s*:\\s*\"(?<tag>[^\"]+)\"",
                RegexOptions.IgnoreCase);
            string latestVersionRaw = m.Success ? m.Groups["tag"].Value : string.Empty;
            string latestVersion = string.IsNullOrEmpty(latestVersionRaw)
                ? string.Empty
                : Regex.Replace(latestVersionRaw, "[^0-9.]", string.Empty);

            return latestVersion;
        }

        public static async void CheckForUpdate()
        {
            string currentVersion = MyPluginInfo.PLUGIN_VERSION;
            string latestVersion;
            try
            {
                latestVersion = await GetLatestVersionAsync();
            }
            catch (Exception ex)
            {
                Notification.Send(MyPluginInfo.PLUGIN_NAME, $"Update Check Failed:\n\"{ex.Message}\"");
                return;
            }

            if (!string.IsNullOrEmpty(latestVersion) && latestVersion != currentVersion)
            {
                Notification.Send(MyPluginInfo.PLUGIN_NAME, $"A new version of {MyPluginInfo.PLUGIN_NAME} <b>{latestVersion}</b> is available.\nYou are currently using version <b>{MyPluginInfo.PLUGIN_VERSION}</b>.");
            }
        }

        public static void OpenGitHubPage()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = GitHubRepoUrl,
                    UseShellExecute = true
                });
                Notification.Send(MyPluginInfo.PLUGIN_NAME, "Opening GitHub page in default browser...");
            }
            catch (Exception ex)
            {
                Notification.Send(MyPluginInfo.PLUGIN_NAME, $"Failed to open GitHub page:\n\"{ex.Message}\"");
            }
        }
    }
}
