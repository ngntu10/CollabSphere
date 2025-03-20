namespace CollabSphere.Helpers;

public static class ParseGuidString
{
    public static Guid ParseGuid(string? guid)
    {
        return Guid.TryParse(guid ?? string.Empty, out var result) ? result : Guid.Empty;
    }

    public static string ParseString(Guid? guid)
    {
        return guid?.ToString() ?? string.Empty;
    }
}
