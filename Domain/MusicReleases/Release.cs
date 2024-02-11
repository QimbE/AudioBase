using System.Dynamic;
using Domain.Abstractions;
using Domain.Artists;
using Domain.Junctions;
using Domain.Labels;
using Domain.Tracks;
using Throw;

namespace Domain.MusicReleases;

/// <summary>
/// Release of some type
/// </summary>
/// <param name="name"> Release name </param>
/// <param name="coverLink"> Link to release cover </param>
/// <param name="authorId"> Id of release`s author </param>
/// <param name="releaseTypeid"> Id of release`s type </param>
/// <param name="releaseDate"> Day of release </param>
public class Release
    : Entity<Release>
{
    private string _name;

    public string Name
    {
        get => _name;
        protected set
        {
            _name = value.Throw()
                .IfNullOrWhiteSpace(n => n);
        }
    }
    
    private string _coverLink;

    public string CoverLink
    {
        get => _coverLink;
        protected set
        {
            _coverLink = value.Throw()
                .IfNullOrWhiteSpace(cl => cl);
        }
    }
    
    private Guid _authorId;

    public Guid AuthorId
    {
        get => _authorId;
        protected set => 
            _authorId = value.Throw()
                .IfNull(ai => ai);
    }
    
    private int _releaseTypeId;
    
    public int ReleaseTypeId
    {
        get => _releaseTypeId;
        protected set
        {
            // if there is no such release type exception is thrown
            _releaseTypeId = value.Throw().IfFalse(rti => ReleaseType.TryFromValue(value, out var _));
        }
    }

    private DateOnly _releaseDate;

    public DateOnly ReleaseDate
    {
        get => _releaseDate;
        protected set
        {
            _releaseDate = value.Throw()
                .IfNotType<DateOnly>()
                .IfDefault(rd => rd);
        }
    }
    
    public Artist Author { get; protected set; }
    
    public ReleaseType ReleaseType { get; protected set; }

    public List<LabelRelease> LabelReleases { get; protected set; }
    
    protected Release()
    {
        
    }

    protected Release(string name, string coverLink, Guid authorId, int releaseTypeId, DateOnly releaseDate)
        : base(Guid.NewGuid())
    {
        Name = name;
        CoverLink = coverLink;
        AuthorId = authorId;
        ReleaseTypeId = releaseTypeId;
        ReleaseDate = releaseDate;
    }

    public static Release Create(string name, string coverLink, Guid authorId, int releaseTypeId, DateOnly releaseDate)
    {
        return new Release(name, coverLink, authorId, releaseTypeId, releaseDate);
    }
    
    public void Update(string name, string coverLink, Guid authorId, int releaseTypeId, DateOnly releaseDate)
    {
        Name = name;
        CoverLink = coverLink;
        AuthorId = authorId;
        ReleaseTypeId = releaseTypeId;
        ReleaseDate = releaseDate;
    }
}