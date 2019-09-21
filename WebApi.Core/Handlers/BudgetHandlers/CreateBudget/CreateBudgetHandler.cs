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
    public class CreateBudgetHandler : BaseBudgetHandler<CreateBudgetRequest, BudgetDetailsDto>
    {

        public CreateBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        public override async Task<BudgetDetailsDto> Handle(CreateBudgetRequest request, CancellationToken cancellationToken)
        {
            request.Data.OwnedByUser = AuthenticationProvider.User;
            var budgetEntity = Mapper.Map<Budget>(request.Data);
            var savedBudget = await BudgetRepository.AddAsync(budgetEntity);

            var addedRows = await BudgetRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(budgetEntity), budgetEntity);
            }

            return  Mapper.Map<BudgetDetailsDto>(savedBudget);
        }
    }
}