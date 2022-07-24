namespace Common.Helpers;

public static class ConnectionStringConverter
{
    public static string ConvertFromUrl(string databaseUrl)
    {
        var separators = new [] {':', '/', '@'};
        var splited = databaseUrl.Split(separators);
        var user = splited[3];
        var password = splited[4];
        var host = splited[5];
        var database = splited[7];
        var connectionString = $"Host={host};Database={database};Username={user};Password={password}";

        return connectionString;
    }
}