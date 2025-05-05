#if !NET40
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HandyControl.Tools;

[JsonSourceGenerationOptions()]
[JsonSerializable(typeof(List<UpdateInfo>))]
internal partial class UpdateHelperJsonContext : JsonSerializerContext
{
}
#endif
