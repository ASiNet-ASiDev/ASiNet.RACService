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
        _pressKey = BaseClient.OpenHandler<PressKeyRequest, PressKeyResponce>() ??
            throw new Exception();
        _upKey = BaseClient.OpenHandler<UpKeyRequest, UpKeyResponce>() ??
            throw new Exception();

        _clickKey = BaseClient.OpenHandler<ClickKeyRequest, ClickKeyResponce>() ??
            throw new Exception();
        _clickOne = BaseClient.OpenHandler<ClickOneRequest, ClickOneResponce>() ??
            throw new Exception();
        _clickTwo = BaseClient.OpenHandler<ClickTwoRequest, ClickTwoResponce>() ??
            throw new Exception();
        _cliclThree = BaseClient.OpenHandler<ClickThreeRequest, ClickThreeResponce>() ??
            throw new Exception();

        _longClick = BaseClient.OpenHandler<LongClickRequest, LongClickResponse>() ??
            throw new Exception();
        _repeatClick = BaseClient.OpenHandler<RepeatClickRequest, RepeatClickResponse>() ??
            throw new Exception();

        BaseClient.ClientDisconnected += OnDisconnected;
    }

    public bool Authorized { get; private set; }


    public ExtensionTcpClient BaseClient { get; init; }

    private PackageHandler<AuthRequest, AuthResponse> _authHandler;
    private PackageHandler<KeyboardAccessRequest, KeyboardAccessResponse> _kbHandler;

    private PackageHandler<PressKeyRequest, PressKeyResponce> _pressKey;
    private PackageHandler<UpKeyRequest, UpKeyResponce> _upKey;

    private PackageHandler<ClickKeyRequest, ClickKeyResponce> _clickKey;
    private PackageHandler<ClickOneRequest, ClickOneResponce> _clickOne;
    private PackageHandler<ClickTwoRequest, ClickTwoResponce> _clickTwo;
    private PackageHandler<ClickThreeRequest, ClickThreeResponce> _cliclThree;

    private PackageHandler<LongClickRequest, LongClickResponse> _longClick;
    private PackageHandler<RepeatClickRequest, RepeatClickResponse> _repeatClick;

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

    public async Task<bool> SendKeyboardEvent(KeyboardAccessRequest request, CancellationToken token = default)
    {
        try
        {
            var result = await _kbHandler.SendAndWaitAccept(request, token);

            if(result is null)
                return false;

            return result.ResponseCode == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendPressKeyEvent(VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _pressKey.SendAndWaitAccept(new(key), token);

            if(result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendUpKeyEvent(VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _upKey.SendAndWaitAccept(new(key), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendClickKeyEvent(VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _clickKey.SendAndWaitAccept(new(key), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendClickOneModifierEvent(VKCode mod, VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _clickOne.SendAndWaitAccept(new(mod, key), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendClickTwoModifierEvent(VKCode mod, VKCode mod1, VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _clickTwo.SendAndWaitAccept(new(mod, mod1, key), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendClickThreeModifierEvent(VKCode mod, VKCode mod1, VKCode mod2, VKCode key, CancellationToken token = default)
    {
        try
        {
            var result = await _cliclThree.SendAndWaitAccept(new(mod, mod1, mod2, key), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendLongClickEvent(VKCode key, int duration, CancellationToken token = default)
    {
        try
        {
            var result = await _longClick.SendAndWaitAccept(new(key, duration), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendRepeatClickByTimeEvent(VKCode key, int interval, int duration, CancellationToken token = default)
    {
        try
        {
            var result = await _repeatClick.SendAndWaitAccept(new(key, interval, duration, null), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendRepeatClickByCountEvent(VKCode key, int interval, int count, CancellationToken token = default)
    {
        try
        {
            var result = await _repeatClick.SendAndWaitAccept(new(key, interval, null, count), token);

            if (result is null)
                return false;

            return result.Code == RACResponseCode.Success;
        }
        catch
        {
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
            BaseClient.CloseHandler(_upKey);
            BaseClient.CloseHandler(_pressKey);
            BaseClient.CloseHandler(_clickKey);
            BaseClient.CloseHandler(_clickOne);
            BaseClient.CloseHandler(_clickTwo);
            BaseClient.CloseHandler(_cliclThree);
            BaseClient.CloseHandler(_longClick);
            BaseClient.CloseHandler(_repeatClick);
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
