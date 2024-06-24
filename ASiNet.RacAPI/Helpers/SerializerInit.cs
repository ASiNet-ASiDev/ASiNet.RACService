using ASiNet.Data.Serialization.V2;
using ASiNet.RacAPI.Packages;

namespace ASiNet.RacAPI.Helpers;
public static class SerializerInit
{
    public static void Init(ISerializerBuilder<ushort> builder) => builder
        .RegisterType<AuthRequest>()
        .RegisterType<AuthResponse>()
        .RegisterType<KeyboardAccessRequest>()
        .RegisterType<KeyboardAccessResponse>();
}
