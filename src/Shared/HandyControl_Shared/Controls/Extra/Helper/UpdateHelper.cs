using HandyControl.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HandyControl.Controls
{
    public class UpdateHelper
    {
        public static string ChangeLog = string.Empty;
        public static string URL = "";

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
        /// <param name="UpdateServerUrl">uploaded xml file url example: https://test.com/myxml.xml
        /// </param>
        /// <returns>True: There is a New Version Available
        /// False: You are using Latest version
        /// </returns>
        public static bool IsNewVersionExist(string UpdateServerUrl)
        {
            ChangeLog = string.Empty;
            URL = string.Empty;

            XDocument doc = XDocument.Load(UpdateServerUrl);
            var items = doc
                .Elements(XName.Get(UpdateXmlChildTag));
            var versionItem = items.Select(ele => ele.Element(XName.Get(UpdateVersionTag)).Value);
            var urlItem = items.Select(ele => ele.Element(XName.Get(UpdateUrlTag)).Value);
            var changelogItem = items.Select(ele => ele.Element(XName.Get(UpdateChangeLogTag)).Value);

            URL = urlItem.FirstOrDefault();
            ChangeLog = changelogItem.FirstOrDefault();

            var newInfo = getSystemVersion(versionItem.FirstOrDefault());
            var oldInfo = getSystemVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var newVer = new SystemVersionInfo(newInfo.Major, newInfo.Minor, newInfo.Build, newInfo.Revision);
            var oldVer = new SystemVersionInfo(oldInfo.Major, oldInfo.Minor, oldInfo.Build, oldInfo.Revision);

            if (newVer > oldVer)
                return true;
            else
                return false;
        }

        private static SystemVersionInfo getSystemVersion(string version)
        {
            var nums = version.Split('.').Select(int.Parse).ToList();
            return new SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
        }
    }
}
