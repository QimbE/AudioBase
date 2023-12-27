﻿using Application.DataAccess;
using Domain.Users;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace Application.GraphQL;

public class Endpoint
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    // TODO: [Authorize()]
    public async Task<IQueryable<User>> GetUsers(
        [Service(ServiceKind.Resolver)] IApplicationDbContext context,
        CancellationToken cancellationToken
        )
    {
        return context.Users;
    }
}