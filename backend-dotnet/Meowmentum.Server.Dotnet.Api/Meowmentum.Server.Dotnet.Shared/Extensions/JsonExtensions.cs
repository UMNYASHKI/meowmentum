using Newtonsoft.Json;

namespace Meowmentum.Server.Dotnet.Shared.Extensions;

public static class JsonExtensions 
{
    public static T JsonDeserializeOrDefault<T>(this string instance, T defaultValue = default)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(instance);
        }
        catch (ArgumentNullException)
        {
            return defaultValue;
        }
        catch (JsonException)
        {
            return defaultValue;
        }
    }

    public static string JsonSerializeOrDefault<T>(this T instance, string defaultValue = default)
    {
        try
        {
            return JsonConvert.SerializeObject(instance);
        }
        catch (ArgumentNullException)
        {
            return defaultValue;
        }
        catch (JsonException)
        {
            return defaultValue;
        }
    }
}
