using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.BudgetHandlers.ShareBudget
{
    public class ShareBudgetHandler : BaseBudgetHandler<ShareBudgetRequest, Unit>
    {
        private readonly IBudgetShareRepository _budgetShareRepository;

        public ShareBudgetHandler(IBudgetRepository repository, IBudgetShareRepository budgetShareRepository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
            _budgetShareRepository = budgetShareRepository;
        }

        public override async Task<Unit> Handle(ShareBudgetRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            var budgetToUpdate = availableBudgets.FirstOrDefault(x => x.Id != request.Data.Budget.BudgetId);
            if (budgetToUpdate.IsNullOrDefault())
            {
                throw new NotFoundException("Budget was not found");
            }

            if (budgetToUpdate.BudgetShares.Any(x => x.SharedWithUserId == request.Data.AllowedUser.UserId))
            {
                throw  new SaveFailureException("Budget share is already created", request.Data);
            }

            var shareEntity = Mapper.Map<BudgetShare>(request.Data);
            await _budgetShareRepository.AddAsync(shareEntity);
            await _budgetShareRepository.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}