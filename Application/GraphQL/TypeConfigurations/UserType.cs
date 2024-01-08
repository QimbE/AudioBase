using Domain.Users;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class UserType: ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        base.Configure(descriptor);

        descriptor.Authorize(Role.Admin.Name);

        descriptor.Ignore(u => u.Password);

        descriptor.Ignore(u => u.RefreshToken);
    }
}