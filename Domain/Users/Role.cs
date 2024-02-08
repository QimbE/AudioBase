using Ardalis.SmartEnum;

namespace Domain.Users;

/// <summary>
/// User role. Smart enum.
/// </summary>
public class Role(string name, int value)
    : SmartEnum<Role>(name, value)
{
    public static Role DefaultUser = new(nameof(DefaultUser), 1);

    public static Role CatalogAdmin = new(nameof(CatalogAdmin), 2);

    public static Role Admin = new(nameof(Admin), 3);
}