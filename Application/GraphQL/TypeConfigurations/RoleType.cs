using Domain.Users;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class RoleType: EnumType<Role>
{
    protected override void Configure(IEnumTypeDescriptor<Role> descriptor)
    {
        foreach (var role in Role.List)
        {
            descriptor.Value(role).Name(role.Name);
        }
    }
}