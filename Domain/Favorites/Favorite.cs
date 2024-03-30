using Domain.Abstractions;
using Domain.Artists;
using Domain.Junctions;
using Domain.Tracks;
using Domain.Users;
using Throw;

namespace Domain.Favorites;

/// <summary>
/// Junction entity for adding tracks by users
/// </summary>
/// <param name="userId"> Id of a user </param>
/// <param name="trackId"> Id of a track to add </param>
public class Favorite: Entity <Favorite>
{
    private Guid _userId;

    public Guid UserId
    {
        get => _userId;
        protected set => 
            _userId = value.Throw()
                .IfNull(ui => ui);
    }
    
    private Guid _trackId;
    
    public Guid TrackId
    {
        get => _trackId;
        protected set => 
            _trackId = value.Throw()
                .IfNull(ti => ti);
    }

    public User User { get; set; }
    
    public Track Track { get; set; }
    
    protected Favorite()
    {
        
    }

    protected Favorite(Guid userId, Guid trackId)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TrackId = trackId;
    }

    public static Favorite Create(Guid userId, Guid trackId)
    {
        return new(userId, trackId);
    }
    // Update method is useless for this entity
}