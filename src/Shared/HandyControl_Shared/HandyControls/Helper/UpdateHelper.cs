#if !NET40
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Net.Http;
using System.Text;
#if NETCOREAPP
using System.Text.Json;
#else
using System.Web.Script.Serialization;
#endif
using System.Threading.Tasks;
using HandyControl.Data;

namespace HandyControl.Tools
{
    public class UpdateHelper
    {
        public static UpdateHelper Instance => new UpdateHelper();

        private SystemVersionInfo GetSystemVersion(string version)
        {
            string removedchar = RemoveExtraText(version);
            var nums = removedchar.Split('.').Select(int.Parse).ToList();

            if (nums.Count <= 3)
            {
                return new SystemVersionInfo(nums[0], nums[1], nums[2], 0);
            }

            return new SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
        }

        private string RemoveExtraText(string version)
        {
            var allowedChars = "01234567890.";
            return new string(version.Where(c => allowedChars.Contains(c)).ToArray());
        }

        public async Task<GithubModel> CheckUpdateAsync(string username, string repository)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(repository))
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", username);
                string url = $"https://api.github.com/repos/{username}/{repository}/releases/latest";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
#if NETCOREAPP
                var result = JsonSerializer.Deserialize<RootModel>(responseBody);
#else
                JavaScriptSerializer javaScript = new JavaScriptSerializer();
                var result = javaScript.Deserialize<RootModel>(responseBody);
#endif
                if (result != null)
                {
                    var newInfo = GetSystemVersion(result.tag_name);
                    var oldInfo = GetSystemVersion(Assembly.GetCallingAssembly().GetName().Version?.ToString());
                    var newVer = new SystemVersionInfo(newInfo.Major, newInfo.Minor, newInfo.Build, newInfo.Revision);
                    var oldVer = new SystemVersionInfo(oldInfo.Major, oldInfo.Minor, oldInfo.Build, oldInfo.Revision);

                    var model = new GithubModel
                    {
                        Changelog = result?.body,
                        CreatedAt = Convert.ToDateTime(result?.created_at),
                        Asset = result.assets,
                        IsPreRelease = result.prerelease,
                        PublishedAt = Convert.ToDateTime(result?.published_at),
                        TagName = result.tag_name,
                        ApiUrl = result?.url,
                        ReleaseUrl = result?.html_url,
                        IsExistNewVersion = newVer > oldVer
                    };


                    return model;
                }
            }
            else
            {
                throw new Exception("Username and Repository can not be empty.");
            }

            return new GithubModel();
        }

        public GithubModel CheckUpdate(string username, string repository)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(repository))
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072;
                string url = $"https://api.github.com/repos/{username}/{repository}/releases/latest";
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.UserAgent = username;
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
#if NETCOREAPP
                    var result = System.Text.Json.JsonSerializer.Deserialize<RootModel>(reader.ReadToEnd());
#else
                    JavaScriptSerializer javaScript = new JavaScriptSerializer();
                    var result = javaScript.Deserialize<RootModel>(reader.ReadToEnd());
#endif
                    if (result != null)
                    {
                        var newInfo = GetSystemVersion(result.tag_name);
                        var oldInfo = GetSystemVersion(Assembly.GetCallingAssembly().GetName().Version?.ToString());
                        var newVer = new SystemVersionInfo(newInfo.Major, newInfo.Minor, newInfo.Build, newInfo.Revision);
                        var oldVer = new SystemVersionInfo(oldInfo.Major, oldInfo.Minor, oldInfo.Build, oldInfo.Revision);

                        var model = new GithubModel
                        {
                            Changelog = result?.body,
                            CreatedAt = Convert.ToDateTime(result?.created_at),
                            Asset = result.assets,
                            IsPreRelease = result.prerelease,
                            PublishedAt = Convert.ToDateTime(result?.published_at),
                            TagName = result.tag_name,
                            ApiUrl = result?.url,
                            ReleaseUrl = result?.html_url,
                            IsExistNewVersion = newVer > oldVer
                        };

                        return model;
                    }
                }
            }
            else
            {
                throw new Exception("Username and Repository can not be empty.");
            }

            return new GithubModel();
        }
        public class Asset
        {
            public int size { get; set; }
            public string browser_download_url { get; set; }
        }
        private class RootModel
        {
            public string url { get; set; }
            public string html_url { get; set; }
            public string tag_name { get; set; }
            public bool prerelease { get; set; }
            public DateTime created_at { get; set; }
            public DateTime published_at { get; set; }
            public List<Asset> assets { get; set; }
            public string body { get; set; }
        }
        public class GithubModel
        {
            public bool IsExistNewVersion { get; internal set; }
            public string ApiUrl { get; internal set; }
            public string ReleaseUrl { get; internal set; }
            public string Changelog { get; internal set; }
            public string TagName { get; internal set; }
            public List<Asset> Asset { get; internal set; }
            public bool IsPreRelease { get; internal set; }
            public DateTime CreatedAt { get; internal set; }
            public DateTime PublishedAt { get; internal set; }
        }
    }
}
#endif
