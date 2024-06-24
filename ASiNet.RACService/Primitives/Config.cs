using System.Text.Json;

namespace ASiNet.RACService.Primitives;
public class Config
{

    private static string _cnfDirPath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ASiDev", "RAC Service");
    private static string _cngPath => Path.Join(_cnfDirPath, "config.json");

    public int Port { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public bool AllowRemoteKeyboardAccess { get; set; }

    public bool AllowRemoteDriveAccess { get; set; }

    public bool AllowRemoteDriveWriteAccess { get; set; }

    public bool AllowDatabaseRemoteAccess { get; set; }

    public bool AllowDatabaseRemoteWrite { get; set; }

    public static Config Read()
    {
        if (!Directory.Exists(_cnfDirPath))
        {
            var def = Config.Default;
            Directory.CreateDirectory(_cnfDirPath);
            using var file = File.Create(_cngPath);
            JsonSerializer.Serialize(file, def);
            return def;
        }
        else
        {
            if (File.Exists(_cngPath))
            {
                using var file = File.OpenRead(_cngPath);
                var cnf = JsonSerializer.Deserialize<Config>(file);
                return cnf ?? throw new NullReferenceException("Config damaged :(");
            }
            else
            {
                var def = Config.Default;
                using var file = File.Create(_cngPath);
                JsonSerializer.Serialize(file, def);
                return def;
            }
        }
    }


    public static Config Default => new()
    {
         Port = 45454,
         Login = null,
         Password = null, 
         AllowDatabaseRemoteAccess = false, 
         AllowDatabaseRemoteWrite = false,
         AllowRemoteDriveWriteAccess = false,
         AllowRemoteDriveAccess = false,
         AllowRemoteKeyboardAccess = true,
    };

}
