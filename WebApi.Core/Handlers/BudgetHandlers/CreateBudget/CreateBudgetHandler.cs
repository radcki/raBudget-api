using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.BudgetHandlers.CreateBudget
{
    public class CreateBudgetHandler : BaseBudgetHandler<CreateBudgetRequest, BudgetDto>
    {

        public CreateBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        public override async Task<BudgetDto> Handle(CreateBudgetRequest request, CancellationToken cancellationToken)
        {
            request.Data.OwnedByUser = AuthenticationProvider.User;
            var budgetEntity = Mapper.Map<Budget>(request.Data);
            budgetEntity.OwnedByUser = null;
            var savedBudget = await BudgetRepository.AddAsync(budgetEntity);

            var addedRows = await BudgetRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(budgetEntity), budgetEntity);
            }

            return  Mapper.Map<BudgetDto>(savedBudget);
        }
    }
}