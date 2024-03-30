using System.Data;

namespace Domain.Labels.Exceptions;

public class LabelWithSameNameException: DuplicateNameException
{
    public LabelWithSameNameException()
        : base("Label with given name already exists")
    {
        
    }
}