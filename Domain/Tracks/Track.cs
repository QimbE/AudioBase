using Domain.Abstractions;
using Domain.Artists;
using Domain.Junctions;
using Domain.MusicReleases;
using Throw;

namespace Domain.Tracks;

/// <summary>
/// Track entity
/// </summary>
/// <param name="id"> Inherited from base "Entity" class </param>
/// <param name="name"> Name of the track </param>
/// <param name="audioLink"> Link to the audio file </param>
/// <param name="duration"> Duration of a track </param>
/// <param name="releaseId"> ID of release, which this track belongs to </param>
/// <param name="genreId"> ID of genre, which this track belongs to </param>
public class Track
    : Entity<Track>
{
    // Track name
    private string _name;

    public string Name
    {
        get => _name;
        protected set
        {
            _name = value.Throw()
                .IfNullOrWhiteSpace(x => x);
        }
    }
    
    // Link to audio file
    // TODO: Choose a way to store files and add default link verification 
    private string _audioLink;

    public string AudioLink
    {
        get => _audioLink;
        protected set
        {
            _audioLink = value.Throw()
                .IfNullOrWhiteSpace(x => x);
        }
    }
    
    // TimeSpan is chosen because most of dll`s return TimeSpan from reading a file header
    private TimeSpan _duration;

    public TimeSpan Duration
    {
        get => _duration;
        protected set
        {
            _duration = value.Throw()
                .IfNotType<TimeSpan>()
                .IfDefault(d => d);
        }
    }
    
    private Guid _releaseId;
    
    public Guid ReleaseId
    {
        get => _releaseId;
        protected set => 
            _releaseId = value.Throw()
                .IfNull(ri => ri);
    }
    
    private Guid _genreId;
    
    public Guid GenreId
    {
        get => _genreId;
        protected set => 
            _genreId = value.Throw()
                .IfNull(gi => gi);
    }
    
    public Genre Genre { get; protected set; }
    
    public Release Release { get; protected set; }
    
    public List<CoAuthor> CoAuthors { get; protected set; }
    
    public Artist Author
    {
        get => Release.Author;
        protected set => Author = value;
    }
    
    protected Track()
    {
        
    }
    
    // ID generates via method from the base class
    protected Track(string name, string audioLink, TimeSpan duration, Guid releaseId, Guid genreId)
        : base(Guid.NewGuid())
    {
        Name = name;
        AudioLink = audioLink;
        Duration = duration;
        ReleaseId = releaseId;
        GenreId = genreId;
    }

    public static Track Create(string name, string audioLink, TimeSpan duration, Guid releaseId, Guid genreId)
    {
        return new(name, audioLink, duration, releaseId, genreId);
    }

    public void Update(string name, string audioLink, TimeSpan duration, Guid releaseId, Guid genreId)
    {
        Name = name;
        AudioLink = audioLink;
        Duration = duration;
        ReleaseId = releaseId;
        GenreId = genreId;
    }
}