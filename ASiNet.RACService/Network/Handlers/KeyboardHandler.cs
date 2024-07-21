using ASiNet.RacAPI.Enums;
using ASiNet.RacAPI.Packages;
using ASiNet.RACService.Primitives;
using ASiNet.Win.IO;

namespace ASiNet.RACService.Network.Handlers;
internal class KeyboardHandler : IDisposable
{
    public KeyboardHandler(RAClient client, Config config)
    {
        _client = client;
        _config = config;
        _client.BaseClient.Subscribe<PressKeyRequest>(PressKey);
        _client.BaseClient.Subscribe<UpKeyRequest>(UpKey);
        _client.BaseClient.Subscribe<ClickKeyRequest>(ClickKey);
        _client.BaseClient.Subscribe<ClickOneRequest>(ClickOneModifier);
        _client.BaseClient.Subscribe<ClickTwoRequest>(ClickTwoModifiers);
        _client.BaseClient.Subscribe<ClickThreeRequest>(ClickThreeModifiers);
        _client.BaseClient.Subscribe<LongClickRequest>(LongClick);
        _client.BaseClient.Subscribe<RepeatClickRequest>(RepeatClick);
    }

    private readonly Config _config;
    private readonly RAClient _client;

    public void PressKey(PressKeyRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new PressKeyResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.PressKey((Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new PressKeyResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new PressKeyResponce(RACResponseCode.Failed));
        }
    }

    public void UpKey(UpKeyRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new UpKeyResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.UpKey((Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new UpKeyResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new UpKeyResponce(RACResponseCode.Failed));
        }
    }

    public void ClickKey(ClickKeyRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new ClickKeyResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.ClickKey((Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new ClickKeyResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new ClickKeyResponce(RACResponseCode.Failed));
        }
    }

    public void ClickOneModifier(ClickOneRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new ClickOneResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.ClickOneModifier((Win.IO.VKCode)request.Mod, (Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new ClickOneResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new ClickOneResponce(RACResponseCode.Failed));
        }
    }

    public void ClickTwoModifiers(ClickTwoRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new ClickTwoResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.ClickTwoModifiers((Win.IO.VKCode)request.Mod, (Win.IO.VKCode)request.Mod2, (Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new ClickTwoResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new ClickTwoResponce(RACResponseCode.Failed));
        }
    }

    public void ClickThreeModifiers(ClickThreeRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new ClickThreeResponce(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.ClickThreeModifiers((Win.IO.VKCode)request.Mod, (Win.IO.VKCode)request.Mod2, (Win.IO.VKCode)request.Mod3, (Win.IO.VKCode)request.Key);
            _client.BaseClient.Send(new ClickThreeResponce(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new ClickThreeResponce(RACResponseCode.Failed));
        }
    }

    public void LongClick(LongClickRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new LongClickResponse(RACResponseCode.AccessDenied));
                return;
            }
            KeyboardInput.LongClick((Win.IO.VKCode)request.Key, request.Duration);
            _client.BaseClient.Send(new LongClickResponse(RACResponseCode.Success));
        }
        catch
        {
            _client.BaseClient.Send(new LongClickResponse(RACResponseCode.Failed));
        }
    }

    public void RepeatClick(RepeatClickRequest request)
    {
        try
        {
            if (!IsAllowAccess())
            {
                _client.BaseClient.Send(new RepeatClickResponse(RACResponseCode.AccessDenied));
                return;
            }
            if (request.Duration.HasValue)
            {
                KeyboardInput.RepeatClickByTime((Win.IO.VKCode)request.Key, request.Duration.Value, request.Interval);
                _client.BaseClient.Send(new ClickThreeResponce(RACResponseCode.Success));
            }
            else if (request.Count.HasValue)
            {
                KeyboardInput.RepeatClickByCount((Win.IO.VKCode)request.Key, request.Count.Value, request.Interval);
                _client.BaseClient.Send(new ClickThreeResponce(RACResponseCode.Success));
            }
            else
            {
                _client.BaseClient.Send(new RepeatClickResponse(RACResponseCode.IncorrectData));
            }
        }
        catch
        {
            _client.BaseClient.Send(new RepeatClickResponse(RACResponseCode.Failed));
        }
    }

    private bool IsAllowAccess()
    {
        if (!_config.AllowRemoteKeyboardAccess || !_client.Authorized)
        {
            return false;
        }
        return true;
    }

    public void Dispose()
    {
        _client.BaseClient.Unsubscribe<PressKeyRequest>(PressKey);
        _client.BaseClient.Unsubscribe<UpKeyRequest>(UpKey);
        _client.BaseClient.Unsubscribe<ClickKeyRequest>(ClickKey);
        _client.BaseClient.Unsubscribe<ClickOneRequest>(ClickOneModifier);
        _client.BaseClient.Unsubscribe<ClickTwoRequest>(ClickTwoModifiers);
        _client.BaseClient.Unsubscribe<ClickThreeRequest>(ClickThreeModifiers);
        _client.BaseClient.Unsubscribe<LongClickRequest>(LongClick);
        _client.BaseClient.Unsubscribe<RepeatClickRequest>(RepeatClick);
    }
}
