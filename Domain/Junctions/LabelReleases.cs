using Domain.Abstractions;
using Throw;

namespace Domain.Junctions;

/// <summary>
/// Linking entity between labels and releases
/// </summary>
/// <param name="labelId"> Id of a label </param>
/// <param name="releaseId"> Id of a release under that label</param>
public class LabelReleases
    : Entity<LabelReleases>
{
    private Guid _labelId;

    public Guid LabelId
    {
        get => _labelId;
        protected set => 
            _labelId = value.Throw()
                .IfNull(li => li);
    }
    
    private Guid _releaseId;
    
    public Guid ReleaseId
    {
        get => _releaseId;
        protected set => 
            _releaseId = value.Throw()
                .IfNull(ri => ri);
    }
    
    protected LabelReleases()
    {
        
    }

    protected LabelReleases(Guid labelId, Guid releaseId)
        : base(Guid.NewGuid())
    {
        LabelId = labelId;
        ReleaseId = releaseId;
    }

    public static LabelReleases Create(Guid labelId, Guid releaseId)
    {
        return new(labelId, releaseId);
    }
    // Update method is useless for this entity
}