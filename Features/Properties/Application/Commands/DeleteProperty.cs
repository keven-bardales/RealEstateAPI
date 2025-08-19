using MediatR;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Features.Properties.Application.Commands
{
    public record DeletePropertyCommand(int Id) : IRequest<DeletePropertyResult>;
    public record DeletePropertyResult(bool Success, string Message);

    public class DeletePropertyHandler : IRequestHandler<DeletePropertyCommand, DeletePropertyResult>
    {
        private readonly IPropertyRepository _repository;

        public DeletePropertyHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeletePropertyResult> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (property == null)
            {
                return new DeletePropertyResult(false, $"Property with ID {request.Id} not found");
            }

            await _repository.DeleteAsync(property);
            await _repository.SaveChangesAsync(cancellationToken);

            return new DeletePropertyResult(true, $"Property {request.Id} deleted successfully");
        }
    }
}