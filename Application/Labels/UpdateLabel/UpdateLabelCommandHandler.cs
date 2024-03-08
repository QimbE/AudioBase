using Application.DataAccess;
using Domain.Labels.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Labels.UpdateLabel;

public class UpdateLabelCommandHandler: IRequestHandler<UpdateLabelCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateLabelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
    {
        var labelFromDb = await _context.Labels.SingleOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        
        // if there is no label with given id in DB
        if (labelFromDb is null)
        {
            return new(new LabelNotFoundException(request.Id));
        }
        
        // Data unchanged
        if (labelFromDb.Name == request.Name && labelFromDb.Description == request.Description && labelFromDb.PhotoLink == request.PhotoLink)
        {
            return new(new LabelUnchangedException());
        }
        
        // if label with same name is already in DB
        var labelWithSameName = await _context.Labels.SingleOrDefaultAsync(
            l => l.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);
        if (labelWithSameName is not null && labelWithSameName!=labelFromDb)
        {
            return new(new LabelWithSameNameException());
        }

        // Update label
        labelFromDb.Update(request.Name, request.Description, request.PhotoLink);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}