using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.Budget.Command
{
    public class CreateBudget
    {
        public class Request : IRequest<Response>
        {
            public string Name { get; set; }
            public Currency Currency { get; set; }
            public DateTime StartingDate { get; set; }

            public IEnumerable<BudgetCategoryDto> BudgetCategories { get; set; }

            public Request()
            {
                BudgetCategories = new List<BudgetCategoryDto>();
            }
        }

        public class Response : BaseResponse<BudgetDto>
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Currency).NotEmpty();
                RuleFor(x => x.StartingDate).NotEmpty();
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
                var budgetEntity = Mapper.Map<Domain.Entities.Budget>(request);
                budgetEntity.OwnedByUserId = AuthenticationProvider.User.UserId;
                var savedBudget = await BudgetRepository.AddAsync(budgetEntity);

                var addedRows = await BudgetRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(budgetEntity), budgetEntity);
                }

                var dto = Mapper.Map<BudgetDto>(savedBudget);

                return new Response() {Data = dto};
            }
        }
    }
}