using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    public class TadaRequestValidator : AbstractValidator<TadaRequest>
    {
        public TadaRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class TadaRequest : IRequest<int>
    {
        public int Id { get; set; }
    }

    public class TadaRequestHandler : IRequestHandler<TadaRequest, int>
    {
        public Task<int> Handle(TadaRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(1337);
        }
    }
}
