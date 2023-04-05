using System.Data.SQLite;

namespace GameLanguageTranslator.Utilities;

public sealed class TranslationFromDatabase : IDisposable
{
    private static TranslationFromDatabase _instance = new TranslationFromDatabase();
    private static string _databasePath = Path.Combine(Environment.CurrentDirectory, "translations.db");
    private static SQLiteConnection _connection;

    private TranslationFromDatabase() {
        if (string.IsNullOrEmpty(_databasePath))
        {
            _databasePath = Path.Combine(Environment.CurrentDirectory, "translations.db"); 
        }
        InitializeDatabase();
        InitializeConnection();
    }

    public static TranslationFromDatabase Instance => _instance;

    private static void InitializeDatabase()
    {
        if (!File.Exists(_databasePath))
        {
            SQLiteConnection.CreateFile(_databasePath);
        }

        using var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
        connection.Open();

        using var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Translations (Text TEXT, SourceLanguageCode TEXT, TargetLanguageCode TEXT, TranslatedText TEXT, PRIMARY KEY (Text, SourceLanguageCode, TargetLanguageCode));", connection);

        command.ExecuteNonQuery();

        connection.Close();
    }

    private static void InitializeConnection()
    {
        _connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
        _connection.Open();
    }

    public async Task<string> GetTranslationFromDatabaseAsync(string text, string sourceLanguageCode, string targetLanguageCode)
    {
        using var command = new SQLiteCommand("SELECT TranslatedText FROM Translations WHERE Text=@Text AND SourceLanguageCode=@SourceLanguageCode AND TargetLanguageCode=@TargetLanguageCode;", _connection);
        command.Parameters.AddWithValue("@Text", text.ToLower());
        command.Parameters.AddWithValue("@SourceLanguageCode", sourceLanguageCode.ToLower());
        command.Parameters.AddWithValue("@TargetLanguageCode", targetLanguageCode.ToLower());

        var result = await command.ExecuteScalarAsync();

        return result?.ToString() ?? string.Empty;
    }

    public async Task<string> SearchInTranslatedWordsFromDatabaseAsync(string text, string sourceLanguageCode, string targetLanguageCode)
    {
        using var command = new SQLiteCommand("SELECT TranslatedText FROM Translations WHERE TranslatedText=@TranslatedText AND SourceLanguageCode=@SourceLanguageCode AND TargetLanguageCode=@TargetLanguageCode;", _connection);
        command.Parameters.AddWithValue("@TranslatedText", text);
        command.Parameters.AddWithValue("@SourceLanguageCode", sourceLanguageCode.ToLower());
        command.Parameters.AddWithValue("@TargetLanguageCode", targetLanguageCode.ToLower());

        var result = await command.ExecuteScalarAsync();

        return result?.ToString() ?? string.Empty;
    }

    public async Task AddTranslationToDatabaseAsync(string text, string sourceLanguageCode, string targetLanguageCode, string translatedText)
    {
        using var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
        await connection.OpenAsync();

        using var command = new SQLiteCommand("INSERT INTO Translations (Text, SourceLanguageCode, TargetLanguageCode, TranslatedText) VALUES (@Text, @SourceLanguageCode, @TargetLanguageCode, @TranslatedText);", connection);
        command.Parameters.AddWithValue("@Text", text.ToLower());
        command.Parameters.AddWithValue("@SourceLanguageCode", sourceLanguageCode.ToLower());
        command.Parameters.AddWithValue("@TargetLanguageCode", targetLanguageCode.ToLower());
        command.Parameters.AddWithValue("@TranslatedText", translatedText);

        await command.ExecuteNonQueryAsync();

        await connection.CloseAsync();
    }

    public void Dispose()
    {
        _connection.Close();
        _databasePath = null!;
        _connection = null!;
    }
}