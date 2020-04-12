using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Common;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Features.Budget.Command
{
    public class UpdateBudget
    {
        public class Command : IRequest<Response>
        {
            public int BudgetId { get; set; }
            public string Name { get; set; }
            public CurrencyDto Currency { get; set; }
            public DateTime StartingDate { get; set; }
            public Guid OwnedByUserId { get; set; }

        }

        public class CurrencyDto
        {
            public eCurrency CurrencyCode { get; set; }
            public string Code { get; set; }
        }

        public class Response : BaseResponse<Unit>
        {
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Currency).NotEmpty();
                RuleFor(x => x.StartingDate).NotEmpty();
                RuleFor(x => x.OwnedByUserId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Command, Response>
        {
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                var availableBudgets = await BudgetRepository.ListAvailableBudgets();
                var budgetEntity = availableBudgets.FirstOrDefault(x => x.Id == command.BudgetId);
                if (budgetEntity == null)
                {
                    throw new NotFoundException("Budget was not found");
                }


                budgetEntity.Name = command.Name;
                budgetEntity.CurrencyCode = command.Currency.CurrencyCode;
                budgetEntity.OwnedByUserId = command.OwnedByUserId;

                await BudgetRepository.UpdateAsync(budgetEntity);
                await BudgetRepository.SaveChangesAsync(cancellationToken);

                return new Response() {Data = new Unit()};
            }
        }
    }
}