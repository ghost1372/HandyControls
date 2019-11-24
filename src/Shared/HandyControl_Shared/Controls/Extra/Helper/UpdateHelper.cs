using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
#if !Core
using System.Web.Script.Serialization;
#endif
using System.Xml.Linq;

namespace HandyControl.Controls
{
    public class UpdateHelper
    {
        private const string UpdateXmlChildTag = "AppVersion"; //Defined in Xml file
        private const string UpdateVersionTag = "version"; //Defined in Xml file
        private const string UpdateUrlTag = "url"; //Defined in Xml file
        private const string UpdateChangeLogTag = "changelog"; //Defined in Xml file

        /// <summary>
        /// Check if new version exist or not
        /// xml must contains one element called: AppVersion
        /// and AppVersion must contains 3 items called:
        /// version, url, changelog
        /// </summary>
        /// <param name="UpdateServerUrl">uploaded xml file url</param>
        /// <example>
        /// https://test.com/myxml.xml
        /// <code>
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <AppVersion>
        /// <version>2.2.5772.0</version>
        /// <url>https://github.com/ghost1372/MoalemYar/releases</url>
        /// <changelog>
        /// fixed bug
        /// </changelog>
        /// </AppVersion>
        /// </code>
        /// </example>
        /// <returns></returns>
        public static WebHostModel CheckForUpdate(string UpdateServerUrl)
        {
            XDocument doc = XDocument.Load(UpdateServerUrl);
            var items = doc
                .Elements(XName.Get(UpdateXmlChildTag));
            var versionItem = items.Select(ele => ele.Element(XName.Get(UpdateVersionTag)).Value);
            var urlItem = items.Select(ele => ele.Element(XName.Get(UpdateUrlTag)).Value);
            var changelogItem = items.Select(ele => ele.Element(XName.Get(UpdateChangeLogTag)).Value);

            var model = new WebHostModel 
            { 
                Url = urlItem.FirstOrDefault(),
                Changelog = changelogItem.FirstOrDefault()
            };
            
            var newInfo = GetSystemVersion(versionItem.FirstOrDefault());
            var oldInfo = GetSystemVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var newVer = new SystemVersionInfo(newInfo.Major, newInfo.Minor, newInfo.Build, newInfo.Revision);
            var oldVer = new SystemVersionInfo(oldInfo.Major, oldInfo.Minor, oldInfo.Build, oldInfo.Revision);

            if (newVer > oldVer)
                model.IsExistNewVersion = true;
            else
                model.IsExistNewVersion = false;

            return model;
        }

        /// <summary>
        /// get version as SystemVersionInfo Format
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static SystemVersionInfo GetSystemVersion(string version)
        {
            string removedchar = RemoveExtraText(version);
            var nums = removedchar.Split('.').Select(int.Parse).ToList();

            if (nums.Count <= 3)
                return new SystemVersionInfo(nums[0], nums[1], nums[2], 0);

            return new SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
        }

        /// <summary>
        /// Remove Extra string from version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string RemoveExtraText(string version)
        {
            var allowedChars = "01234567890.";
            return new string(version.Where(c => allowedChars.Contains(c)).ToArray());
        }

        /// <summary>
        /// Check if new version in Github [Release] Exist or Not
        /// </summary>
        /// <param name="Username">Github Username</param>
        /// <param name="Repository">Github Repository</param>
        /// <returns></returns>
        public static GithubReleaseModel CheckForUpdateGithubRelease(string Username, string Repository)
        {
            var rel = SendGithubRestApi(Username, Repository);
            var model = new GithubReleaseModel();

            if(rel !=null)
            {
                model = new GithubReleaseModel
                {
                    Changelog = rel?.body,
                    CreatedAt = Convert.ToDateTime(rel?.created_at),
                    Asset = rel.assets.Any() ? rel?.assets.ToList() : null,
                    IsPreRelease = rel.prerelease,
                    PublishedAt = Convert.ToDateTime(rel?.published_at),
                    Version = rel.tag_name,
                    Url = rel?.url
                };
                var newInfo = GetSystemVersion(rel.tag_name);
                var oldInfo = GetSystemVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                var newVer = new SystemVersionInfo(newInfo.Major, newInfo.Minor, newInfo.Build, newInfo.Revision);
                var oldVer = new SystemVersionInfo(oldInfo.Major, oldInfo.Minor, oldInfo.Build, oldInfo.Revision);
                if (newVer > oldVer)
                {
                    model.IsExistNewVersion = true;
                }
                else
                {
                    model.IsExistNewVersion = false;
                }
            }

            return model;
        }

        /// <summary>
        /// Rest Request to Github API
        /// </summary>
        /// <param name="Username">Github Username</param>
        /// <param name="Repository">Github Repository</param>
        /// <returns></returns>
        internal static Root SendGithubRestApi(string Username, string Repository)
        {
            string url = url = string.Format(
                      "https://api.github.com/repos/{0}/{1}/releases/latest",
                      Username, Repository);

            //Fix Could not create SSL/TLS secure channel
#if netle40
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
#else
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif

             HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                request.UserAgent = Username;

                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
#if !Core
                    JavaScriptSerializer javaScript = new JavaScriptSerializer();
                    return javaScript.Deserialize<Root>(reader.ReadToEnd());
#else
                    return System.Text.Json.JsonSerializer.Deserialize<Root>(reader.ReadToEnd());
#endif
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                return null;
            }
        }

#region Model
#region Model for Deserialize json

        public class Asset
        {
            public int size { get; internal set; }
            public string browser_download_url { get; internal set; }
        }

        internal class Root
        {
            public string url { get; set; }
            public string tag_name { get; set; }
            public bool prerelease { get; set; }
            public DateTime created_at { get; set; }
            public DateTime published_at { get; set; }
            public List<Asset> assets { get; set; }
            public string body { get; set; }
        }
#endregion

        /// <summary>
        /// Model for Return data to user
        /// </summary>
        public class GithubReleaseModel
        {
            public bool IsExistNewVersion { get; internal set; }
            public string Url { get; internal set; }
            public string Changelog { get; internal set; }
            public string Version { get; internal set; }
            public List<Asset> Asset { get; internal set; }
            public bool IsPreRelease { get; internal set; }
            public DateTime CreatedAt { get; internal set; }
            public DateTime PublishedAt { get; internal set; }
        }

        /// <summary>
        /// Model for Return data to user
        /// </summary>
        public class WebHostModel
        {
            public bool IsExistNewVersion { get; set; }
            public string Changelog { get; set; }
            public string Url { get; set; }
        }
#endregion
    }
}
