using Application.DataAccess;
using Domain.Labels;
using Domain.Labels.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Labels.CreateLabel;

public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateLabelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateLabelCommand request, CancellationToken cancellationToken)
    {
        // Label name should be unique
        if (await _context.Labels.AnyAsync(
                l => l.Name.ToLower() == request.Name.ToLower(),
                cancellationToken)
           )
        {
            return new(new LabelWithSameNameException());
        }

        // New Label
        var label = Label.Create(request.Name, request.Description, request.PhotoLink);

        _context.Labels.Add(label);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}