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
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.BudgetHandlers.RevokeBudgetAccess
{
    public class RevokeBudgetShareHandler : BaseBudgetHandler<RevokeBudgetShareRequest, Unit>
    {
        private readonly IBudgetShareRepository _budgetShareRepository;

        public RevokeBudgetShareHandler(IBudgetRepository repository, IBudgetShareRepository budgetShareRepository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
            _budgetShareRepository = budgetShareRepository;
        }

        public override async Task<Unit> Handle(RevokeBudgetShareRequest request, CancellationToken cancellationToken)
        {
            var findShareTask = _budgetShareRepository.ListWithFilter(new Budget(request.Data
                                                                                        .Budget
                                                                                        .BudgetId),
                                                                      new BudgetShareFilterModel()
                                                                      {
                                                                          UserIdFilter = request.Data.AllowedUser.UserId
                                                                      });
            var availableBudgets = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            var budgetToUpdate = availableBudgets.FirstOrDefault(x => x.Id != request.Data.Budget.BudgetId);
            if (budgetToUpdate.IsNullOrDefault())
            {
                throw new NotFoundException("Budget was not found");
            }

            var budgetShares = await findShareTask;
            if (!budgetShares.Any())
            {
                throw new NotFoundException("Budget share was not found");
            }
            await Task.WhenAll(budgetShares.Select(i => _budgetShareRepository.DeleteAsync(i)));
            try
            {
                await _budgetShareRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw new SaveFailureException("budget share", budgetShares);
            }

            return new Unit();
        }
    }
}