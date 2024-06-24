using System.IO;
using System.Text.Json;

namespace ASiNet.RACService.GUI;
public class ServiceConfig
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

    public static ServiceConfig Read()
    {
        if (!Directory.Exists(_cnfDirPath))
        {
            var def = ServiceConfig.Default;
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
                var cnf = JsonSerializer.Deserialize<ServiceConfig>(file);
                return cnf ?? throw new NullReferenceException("Config damaged :(");
            }
            else
            {
                var def = ServiceConfig.Default;
                using var file = File.Create(_cngPath);
                JsonSerializer.Serialize(file, def);
                return def;
            }
        }
    }

    public static bool Write(ServiceConfig config)
    {
        try
        {
            if (!Directory.Exists(_cnfDirPath))
            {
                Directory.CreateDirectory(_cnfDirPath);
            }
            using var file = File.Create(_cngPath);
            JsonSerializer.Serialize(file, config);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public static ServiceConfig Default => new()
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
