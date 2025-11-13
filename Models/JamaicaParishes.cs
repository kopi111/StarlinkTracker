namespace StarlinkTracker.Models;

public static class JamaicaParishes
{
    public static readonly string[] AllParishes = new[]
    {
        "Kingston",
        "St. Andrew",
        "St. Thomas",
        "Portland",
        "St. Mary",
        "St. Ann",
        "Trelawny",
        "St. James",
        "Hanover",
        "Westmoreland",
        "St. Elizabeth",
        "Manchester",
        "Clarendon",
        "St. Catherine"
    };

    public static bool IsValidParish(string parish)
    {
        return AllParishes.Contains(parish, StringComparer.OrdinalIgnoreCase);
    }
}
