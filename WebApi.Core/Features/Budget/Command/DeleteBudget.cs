﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.Budget.Command
{
    public class DeleteBudget
    {
        public class Request : IRequest<Response>
        {
            public int BudgetId { get; set; }
        }

        public class Response : BaseResponse<Unit>
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
            }
        }

        public class Notification : INotification
        {
            public int BudgetId { get; set; }
        }

        public class Handler : BaseBudgetHandler<Request, Response>
        {
            private readonly IMediator _mediator;

            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider, IMediator mediator) : base(repository, mapper, authenticationProvider)
            {
                _mediator = mediator;
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var budgetEntity = await BudgetRepository.GetByIdAsync(request.BudgetId);
                if (AuthenticationProvider.User.UserId != budgetEntity.OwnedByUserId)
                {
                    throw new NotFoundException("Requested budget was not found in user's owned budgets'");
                }

                await BudgetRepository.DeleteAsync(budgetEntity);
                await BudgetRepository.SaveChangesAsync(cancellationToken);
                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = budgetEntity.Id,
                                      }, cancellationToken);
                return new Response() {Data = new Unit()};
            }
        }
    }
}