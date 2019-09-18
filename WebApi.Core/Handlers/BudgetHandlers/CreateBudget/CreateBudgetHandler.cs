using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.CreateBudget
{
    public class CreateBudgetHandler : IRequestHandler<CreateBudgetRequest, CreateBudgetResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public CreateBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<CreateBudgetResponse> Handle(CreateBudgetRequest request, CancellationToken cancellationToken)
        {
            var budgetEntity = _mapper.Map<Budget>(request.Data);
            var savedBudget = await _repository.AddAsync(budgetEntity);

            var addedRows = await _repository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(budgetEntity), budgetEntity);
            }

            return new CreateBudgetResponse
                   {
                       Data = _mapper.Map<BudgetDetailsDto>(savedBudget),
                       ResponseType = eResponseType.Success
                   };
        }
    }
}