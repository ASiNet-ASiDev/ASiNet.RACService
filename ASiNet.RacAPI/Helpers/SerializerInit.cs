using ASiNet.Data.Serialization.V2;
using ASiNet.RacAPI.Packages;

namespace ASiNet.RacAPI.Helpers;
public static class SerializerInit
{
    public static void Init(ISerializerBuilder<ushort> builder) => builder
        .RegisterType<AuthRequest>()
        .RegisterType<AuthResponse>()
        .RegisterType<KeyboardAccessRequest>()
        .RegisterType<KeyboardAccessResponse>()
        .RegisterType<PressKeyRequest>()
        .RegisterType<PressKeyResponce>()
        .RegisterType<UpKeyRequest>()
        .RegisterType<UpKeyResponce>()
        .RegisterType<ClickKeyRequest>()
        .RegisterType<ClickKeyResponce>()
        .RegisterType<ClickOneRequest>()
        .RegisterType<ClickOneResponce>()
        .RegisterType<ClickTwoRequest>()
        .RegisterType<ClickTwoResponce>()
        .RegisterType<ClickThreeRequest>()
        .RegisterType<ClickThreeResponce>()
        .RegisterType<LongClickRequest>()
        .RegisterType<LongClickResponse>()
        .RegisterType<RepeatClickRequest>()
        .RegisterType<RepeatClickResponse>();
}
