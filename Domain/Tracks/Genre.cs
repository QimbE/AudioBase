using Domain.Abstractions;
using Throw;

namespace Domain.Tracks;

/// <summary>
/// Genre of a track
/// </summary>
/// <param name="Id"> Inherited from base "Entity" class </param>
/// <param name="_name"> Name of the genre </param>
public class Genre
    : Entity<Genre>
{
    private string _name;
    public string Name
    {
        get => _name;
        protected set
        {
            _name = value.Throw().IfNullOrWhiteSpace(x => x);
        }
    }

    protected Genre()
    {
        
    }
    
    // ID generates via method from the base class
    protected Genre(string name)
        : base(Guid.NewGuid())
    {
        Name = name;
    }

    public static Genre Create(string name)
    {
        return new(name);
    }

    public void Update(string name)
    {
        Name = name;
    }
}