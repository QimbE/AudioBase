using Domain.Abstractions;
using Throw;

namespace Domain.Labels;

/// <summary>
/// Label under which the album is released
/// </summary>
/// <param name="name"> Label name </param>
/// <param name="description"> Label description </param>
/// <param name="photoLink"> Link to label`s photo </param>
public class Label
    : Entity<Label>
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

    protected Label()
    {
        
    }

    protected Label(string name, string description, string photoLink)
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        PhotoLink = photoLink;
    }

    public static Label Create(string name, string description, string photoLink)
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