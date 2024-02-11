using Ardalis.SmartEnum;
using Domain.Abstractions;
using Domain.Users;
using Throw;

namespace Domain.MusicReleases;

/// <summary>
/// Type of release
/// </summary>
/// <param name="name"> Name of the type </param>
/// <param name="value"> Auto enum value </param>
public class ReleaseType(string name, int value)
    : SmartEnum<ReleaseType>(name, value)
{
    public static ReleaseType Single = new(nameof(Single), 1);
    public static ReleaseType Album = new(nameof(Album), 2);
    public static ReleaseType Bootleg = new(nameof(Bootleg), 3);
    public static ReleaseType EP = new(nameof(EP), 4);
    public static ReleaseType LP = new(nameof(LP), 5);
    public static ReleaseType Live = new(nameof(Live), 6);
    public static ReleaseType Mixtape = new(nameof(Mixtape), 7);
    public static ReleaseType Compilation = new(nameof(Compilation), 8);
}