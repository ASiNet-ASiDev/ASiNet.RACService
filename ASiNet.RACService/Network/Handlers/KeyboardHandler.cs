using ASiNet.RacAPI.Packages;
using ASiNet.RACService.Primitives;
using ASiNet.WinLib.Extensions;

namespace ASiNet.RACService.Network.Handlers;
internal class KeyboardHandler : IDisposable
{
    public KeyboardHandler(RAClient client, Config config)
    {
        _client = client;
        _client.BaseClient.Subscribe<KeyboardAccessRequest>(OnKeyboardAccess);
        _config = config;
    }

    private Config _config;
    private RAClient _client;


    private void OnKeyboardAccess(KeyboardAccessRequest request)
    {
        try
        {
            if (!_config.AllowRemoteKeyboardAccess || !_client.Authorized)
            {
                _client.BaseClient.Send(new KeyboardAccessResponse() { ResponseCode = RacAPI.Enums.RACResponseCode.AccessDenied });
                return;
            }
            switch (request.Type)
            {
                case KeyboardRequestType.SingleKey:
                    WinActionsExtensions.KeyboardAction(request.KeyState switch
                    {
                        KeyState.Click => SingleVirtialKeyEvent.NewKeyClick((WinLib.Enums.VirtualKeyCode)request.KeyCode),
                        KeyState.Up => SingleVirtialKeyEvent.NewKeyUp((WinLib.Enums.VirtualKeyCode)request.KeyCode),
                        KeyState.Down => SingleVirtialKeyEvent.NewKeyDown((WinLib.Enums.VirtualKeyCode)request.KeyCode),
                        _ => throw new NotImplementedException(),
                    });
                    break;
                case KeyboardRequestType.MultikeyOneModifier:
                    WinActionsExtensions.KeyboardAction(new MultiKeyEvent()
                        .SetMod1((WinLib.Enums.VirtualKeyCode)request.Mod1)
                        .SetKey((WinLib.Enums.VirtualKeyCode)request.KeyCode));
                    break;
                case KeyboardRequestType.MultikeyTwoModifiers:
                    WinActionsExtensions.KeyboardAction(new MultiKeyEvent()
                        .SetMod1((WinLib.Enums.VirtualKeyCode)request.Mod1)
                        .SetMod2((WinLib.Enums.VirtualKeyCode)request.Mod2)
                        .SetKey((WinLib.Enums.VirtualKeyCode)request.KeyCode));
                    break;
            }
            _client.BaseClient.Send(new KeyboardAccessResponse() { ResponseCode = RacAPI.Enums.RACResponseCode.Success });
        }
        catch (Exception)
        {
            _client.BaseClient.Send(new KeyboardAccessResponse() { ResponseCode = RacAPI.Enums.RACResponseCode.Failed });
        }
    }

    public void Dispose()
    {
        _client.BaseClient.Unsubscribe<KeyboardAccessRequest>(OnKeyboardAccess);
    }
}
