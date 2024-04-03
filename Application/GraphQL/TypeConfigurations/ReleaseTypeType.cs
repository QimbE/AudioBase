using Domain.MusicReleases;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class ReleaseTypeType : EnumType<Domain.MusicReleases.ReleaseType>
{
    protected override void Configure(IEnumTypeDescriptor<Domain.MusicReleases.ReleaseType> descriptor)
    {
        foreach (var rt in Domain.MusicReleases.ReleaseType.List)
        {
            descriptor.Value(rt).Name(rt.Name);
        }
    }
}