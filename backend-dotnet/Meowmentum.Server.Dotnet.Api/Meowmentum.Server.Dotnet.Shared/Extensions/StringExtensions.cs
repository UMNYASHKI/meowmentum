namespace Meowmentum.Server.Dotnet.Shared.Extensions;

public static class StringExtensions
{
    public static string Append(this string instance, string message, string delimiter = null)
    {
        return string.IsNullOrEmpty(instance) ? message : $"{instance}{delimiter ?? string.Empty}{message}";
    }
}
