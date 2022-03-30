#if !NET40

using System;
using System.Collections.Generic;
#if NETCOREAPP
using System.Text.Json.Serialization;
#else
using System.Runtime.Serialization;
#endif

namespace HandyControl.Tools;

#if NETCOREAPP
public class ReleaseInfo
{
    [JsonPropertyName("url")]
    public string ApiUrl { get; set; }
    [JsonPropertyName("html_url")]
    public string ReleaseUrl { get; set; }
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }
    [JsonPropertyName("prerelease")]
    public bool IsPreRelease { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
    [JsonPropertyName("assets")]
    public List<Asset> Assets { get; set; }
    [JsonPropertyName("body")]
    public string Changelog { get; set; }
    public bool IsExistNewVersion { get; set; }
}

public class Asset
{
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("browser_download_url")]
    public string Url { get; set; }
}

#else
[DataContract]
public class ReleaseInfo
{
    [DataMember(Name = "url")]
    public string ApiUrl { get; set; }
    [DataMember(Name = "html_url")]
    public string ReleaseUrl { get; set; }
    [DataMember(Name = "tag_name")]
    public string TagName { get; set; }
    [DataMember(Name = "prerelease")]
    public bool IsPreRelease { get; set; }
    [DataMember(Name = "created_at")]
    public DateTime CreatedAt { get; set; }
    [DataMember(Name = "published_at")]
    public DateTime PublishedAt { get; set; }
    [DataMember(Name = "assets")]
    public List<Asset> Assets { get; set; }
    [DataMember(Name = "body")]
    public string Changelog { get; set; }
    public bool IsExistNewVersion { get; set; }
}

[DataContract]
public class Asset
{
    [DataMember(Name = "size")]
    public int Size { get; set; }
    [DataMember(Name = "browser_download_url")]
    public string Url { get; set; }
}
#endif
#endif
