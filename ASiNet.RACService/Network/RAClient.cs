using System.Net.Sockets;
using ASiNet.RacAPI.Packages;
using ASiNet.RACService.Network.Handlers;
using ASiNet.RACService.Primitives;
using ASiNet.TcpLib;

namespace ASiNet.RACService.Network;
public class RAClient : IDisposable
{
    public RAClient(TcpClient client, Config config)
    {
        _config = config;
        _client = new(client, RacAPI.Helpers.SerializerInit.Init);
        _client.Subscribe<AuthRequest>(OnAuthRequest);

        _keyboardHandler = new(this, config);
    }

    public bool IsConnected => _client.Connected;

    public bool Authorized => _authorized;

    public ExtensionTcpClient BaseClient => _client;

    private Config _config;
    private ExtensionTcpClient _client;

    private KeyboardHandler _keyboardHandler;

    private byte _authAttempthCount;

    private bool _authorized = false;

    private void OnAuthRequest(AuthRequest request)
    {
        if(_authAttempthCount > 5)
        {
            _client.Send(new AuthResponse()
            {
                ResponseCode = RacAPI.Enums.RACResponseCode.OutOfAttempts,
            });
            Dispose();
            return;
        }
        if(request.Login != _config.Login || request.Password != _config.Password)
        {
            _authAttempthCount++;
            _client.Send(new AuthResponse()
            {
                ResponseCode = RacAPI.Enums.RACResponseCode.IncorrectData
            });
            return;
        }
        var accessDeniedInfo = RacAPI.Enums.RACPermissions.None;
        if (request.PermissionsRequest.HasFlag(RacAPI.Enums.RACPermissions.KeyboardAccess) && !_config.AllowRemoteKeyboardAccess)
            accessDeniedInfo |= RacAPI.Enums.RACPermissions.KeyboardAccess;
        if (request.PermissionsRequest.HasFlag(RacAPI.Enums.RACPermissions.DatabaseAccess) && !_config.AllowDatabaseRemoteAccess)
            accessDeniedInfo |= RacAPI.Enums.RACPermissions.DatabaseAccess;
        if (request.PermissionsRequest.HasFlag(RacAPI.Enums.RACPermissions.DatabaseWriteAccess) && !_config.AllowDatabaseRemoteAccess)
            accessDeniedInfo |= RacAPI.Enums.RACPermissions.DatabaseWriteAccess;
        if (request.PermissionsRequest.HasFlag(RacAPI.Enums.RACPermissions.DriveAccess) && !_config.AllowDatabaseRemoteAccess)
            accessDeniedInfo |= RacAPI.Enums.RACPermissions.DriveAccess;
        if (request.PermissionsRequest.HasFlag(RacAPI.Enums.RACPermissions.DriveWriteAccess) && !_config.AllowDatabaseRemoteAccess)
            accessDeniedInfo |= RacAPI.Enums.RACPermissions.DriveWriteAccess;

        if(accessDeniedInfo != RacAPI.Enums.RACPermissions.None)
        {
            _client.Send(new AuthResponse()
            {
                ResponseCode = RacAPI.Enums.RACResponseCode.AccessDenied,
                AccessDeniedInfo = accessDeniedInfo
            });
            Dispose();
            return;
        }
        _authorized = true;
        _client.Send(new AuthResponse() { ResponseCode = RacAPI.Enums.RACResponseCode.Success });
    }

    public void Dispose()
    {
        _client.Unsubscribe<AuthRequest>(OnAuthRequest);
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
