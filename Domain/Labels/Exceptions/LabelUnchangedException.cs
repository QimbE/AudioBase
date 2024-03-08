using System.Data;

namespace Domain.Labels.Exceptions;

public class LabelUnchangedException: DuplicateNameException
{
    public LabelUnchangedException()
        : base("Label data is unchanged")
    {
        
    }
}