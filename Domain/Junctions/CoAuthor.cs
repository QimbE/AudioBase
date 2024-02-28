using Domain.Abstractions;
using Domain.Artists;
using Domain.Tracks;
using Throw;

namespace Domain.Junctions;

/// <summary>
/// Linking entity between tracks and co-authors
/// </summary>
/// <param name="trackId"> Id of a track </param>
/// <param name="coAuthorId"> Id of a co-author of that track </param>
public class CoAuthor
    : Entity<CoAuthor>
{
    private Guid _trackId;

    public Guid TrackId
    {
        get => _trackId;
        protected set => 
            _trackId = value.Throw()
                .IfNull(ti => ti);
    }
    
    private Guid _coAuthorId;
    
    public Guid CoAuthorId
    {
        get => _coAuthorId;
        protected set => 
            _coAuthorId = value.Throw()
                .IfNull(cai => cai);
    }

    public Artist Artist { get; set; }
    
    public Track Track { get; set; }
    
    protected CoAuthor()
    {
        
    }

    protected CoAuthor(Guid trackId, Guid coAuthorId)
        : base(Guid.NewGuid())
    {
        TrackId = trackId;
        CoAuthorId = coAuthorId;
    }

    public static CoAuthor Create(Guid trackId, Guid coAuthorId)
    {
        return new(trackId, coAuthorId);
    }
    // Update method is useless for this entity
}