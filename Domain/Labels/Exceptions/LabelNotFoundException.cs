using Domain.Abstractions.Exceptions;

namespace Domain.Labels.Exceptions;

public class LabelNotFoundException: EntityNotFoundException<Label>
{
    public LabelNotFoundException(Guid id)
        : base(id)
    {

    }

    public LabelNotFoundException()
        : base("There is no such label in the database")
    {

    }
}