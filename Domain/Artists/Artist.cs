using Domain.Abstractions;
using Throw;

namespace Domain.Artists;

/// <summary>
/// Artist (songwriter, group, producer etc.)
/// </summary>
/// <param name="name"> Name of an artist or of a group of artists </param>
/// <param name="description"> Artist description </param>
/// <param name="photoLink"> Link to artist`s photo </param>
public class Artist
    :Entity<Artist>
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
    
    private string? _description;

    public string Description 
    {
        get => _description;
        // Description is nullable
        protected set => _description = value;
    }
    
    private string _photoLink;
    
    public string PhotoLink
    {
        get => _photoLink;
        protected set => _photoLink = value.Throw().IfNullOrWhiteSpace(pl => pl);
    }
    
    protected Artist()
    {
        
    }

    protected Artist(string name, string description, string photoLink)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        PhotoLink = photoLink;
    }

    public static Artist Create(string name, string description, string photoLink)
    {
        return new(name, description, photoLink);
    }

    public void Update(string name, string description, string photoLink)
    {
        Name = name;
        Description = description;
        PhotoLink = photoLink;
    }
}