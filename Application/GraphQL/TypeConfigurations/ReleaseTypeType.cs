using Domain.MusicReleases;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class ReleaseTypeType : EnumType<ReleaseType>
{
    protected override void Configure(IEnumTypeDescriptor<ReleaseType> descriptor)
    {
        foreach (var rt in ReleaseType.List)
        {
            descriptor.Value(rt).Name(rt.Name);
        }
    }
}