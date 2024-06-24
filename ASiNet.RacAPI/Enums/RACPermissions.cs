namespace ASiNet.RacAPI.Enums;
public enum RACPermissions
{
    None,
    KeyboardAccess = 1 << 0,
    DatabaseAccess = 1 << 1,
    DatabaseWriteAccess = 1 << 2,
    DriveAccess = 1 << 3,
    DriveWriteAccess = 1 << 4,
}
