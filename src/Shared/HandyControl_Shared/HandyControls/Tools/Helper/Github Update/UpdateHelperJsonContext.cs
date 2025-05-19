#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HandyControl.Tools;

[JsonSourceGenerationOptions()]
[JsonSerializable(typeof(List<UpdateInfo>))]
internal partial class UpdateHelperJsonContext : JsonSerializerContext
{
}
#endif
