using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.BudgetHandlers.Query
{
    public class UpdateBudget
    {
        public class Request : IRequest<Response>
        {
            public int BudgetId { get; set; }
            public string Name { get; set; }
            public Currency Currency { get; set; }
            public DateTime StartingDate { get; set; }
            public UserDto OwnedByUser { get; set; }

        }

        public class Response : BaseResponse<Unit>
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Currency).NotEmpty();
                RuleFor(x => x.StartingDate).NotEmpty();
                RuleFor(x => x.OwnedByUser).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Request, Response>
        {
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var availableBudgets = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
                var budgetEntity = availableBudgets.FirstOrDefault(x => x.Id == request.BudgetId);
                if (budgetEntity == null)
                {
                    throw new NotFoundException("Budget was not found");
                }


                budgetEntity.Name = request.Name;
                budgetEntity.CurrencyCode = request.Currency.CurrencyCode;
                budgetEntity.OwnedByUserId = request.OwnedByUser.UserId;

                await BudgetRepository.UpdateAsync(budgetEntity);
                await BudgetRepository.SaveChangesAsync(cancellationToken);

                return new Response() {Data = new Unit()};
            }
        }
    }
}