using Ardalis.SmartEnum;

namespace Domain.Users;

/// <summary>
/// User role. Smart enum.
/// </summary>
public class Role(string name, int value)
    : SmartEnum<Role>(name, value)
{
    public static Role DefaultUser = new(nameof(DefaultUser), 1);
    // TODO: add more roles
}