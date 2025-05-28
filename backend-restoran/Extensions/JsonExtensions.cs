using Newtonsoft.Json;

namespace backend_restoran.Extensions;

public static class JsonExtensions
{
  public static string ToJson(this object obj) =>
    JsonConvert.SerializeObject(obj);

  public static T FromJson<T>(this string json) =>
    JsonConvert.DeserializeObject<T>(json)!;
}