using System.Diagnostics;
using ASiNet.RacAPI.Enums;
using ASiNet.RacAPI.Helpers;
using ASiNet.RacAPI.Packages;
using ASiNet.TcpLib;

namespace ASiNet.RacAPI;
public class RAClient : IDisposable
{
    public RAClient()
    {
        BaseClient = new(SerializerInit.Init);
        _authHandler = BaseClient.OpenHandler<AuthRequest, AuthResponse>() ??
            throw new Exception();
        _kbHandler = BaseClient.OpenHandler<KeyboardAccessRequest, KeyboardAccessResponse>() ?? 
            throw new Exception();
        BaseClient.ClientDisconnected += OnDisconnected;
    }

    public bool Authorized { get; private set; }


    public ExtensionTcpClient BaseClient { get; init; }

    private PackageHandler<AuthRequest, AuthResponse> _authHandler;
    private PackageHandler<KeyboardAccessRequest, KeyboardAccessResponse> _kbHandler;

    public async Task<RACResponseCode> Connect(string address, int port, string? login, string? password, RACPermissions permissions, CancellationToken token)
    {
        try
        {
            if(BaseClient.Connected || await BaseClient.Connect(address, port))
            {
                var result = await _authHandler.SendAndWaitAccept(new AuthRequest() { Login = login, Password = password, PermissionsRequest = permissions }, token);
                if(result is null)
                {
                    Authorized = false;
                    return RACResponseCode.Failed;
                }

                if(result.ResponseCode == RACResponseCode.Success)
                {
                    Authorized = true;
                    return RACResponseCode.Success;
                }
#if DEBUG
                Debug.WriteLine($"{result.ResponseCode} -> {result.AccessDeniedInfo ?? RACPermissions.None}", "RAC::CONNECT");
#endif
                Authorized = false;
                return result.ResponseCode;  
            }
            else
                return RACResponseCode.Failed;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex, "RAC::CONNECT");
#endif
            Authorized = false;
            return RACResponseCode.Failed;
        }
    }

    public async Task<bool> SendKeyboardEvent(KeyboardAccessRequest request, CancellationToken token)
    {
        try
        {
            var result = await _kbHandler.SendAndWaitAccept(request, token);

            if(result is null)
                return false;
#if DEBUG
            Debug.WriteLine($"{result.ResponseCode}", "RAC::SendKeyboardEvent");
#endif
            return result.ResponseCode == RACResponseCode.Success;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex, "RAC::SendKeyboardEvent");
#endif
            return false;
        }
    }

    private void OnDisconnected()
    {
        Authorized = false;
    }

    public void Dispose()
    {
        try
        {
            Authorized = false;
            BaseClient.ClientDisconnected -= OnDisconnected;
            BaseClient.CloseHandler(_authHandler);
            BaseClient.CloseHandler(_kbHandler);
            BaseClient?.Dispose();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex, "RAC::DISPOSE");
#endif
        }
    }
}
