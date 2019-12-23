﻿using System;
using System.Collections.Generic;
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

        public class Handler : BaseBudgetHandler<Request, Response>
        {
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
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

                return new Response() {Data = new Unit()};
            }
        }
    }
}