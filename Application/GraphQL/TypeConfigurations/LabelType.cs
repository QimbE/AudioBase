using Domain.Labels;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class LabelType: EntityType<Label>
{
    protected override void Configure(IObjectTypeDescriptor<Label> descriptor)
    {
        base.Configure(descriptor);
    }
}