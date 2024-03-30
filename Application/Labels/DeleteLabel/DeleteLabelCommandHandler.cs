using Application.DataAccess;
using Domain.Labels.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Labels.DeleteLabel;

public class DeleteLabelCommandHandler: IRequestHandler<DeleteLabelCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteLabelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
    {
        var labelFromDb = await _context.Labels.SingleOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        
        // if there is no label with given id in DB
        if (labelFromDb is null)
        {
            return new(new LabelNotFoundException(request.Id));
        }
        
        // Delete artist
        _context.Labels.Remove(labelFromDb);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}