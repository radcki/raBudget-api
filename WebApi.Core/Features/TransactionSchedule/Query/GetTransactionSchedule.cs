using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.TransactionSchedule.Query
{
    public class GetTransactionSchedule
    {
        public class Query : IRequest<TransactionScheduleDetailsDto>
        {
            public int TransactionScheduleId;

            public Query(int transactionId)
            {
                TransactionScheduleId = transactionId;
            }
        }

        public class GetTransactionScheduleRequestValidator : AbstractValidator<Query>
        {
            public GetTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.TransactionScheduleId).NotEmpty();
            }
        }
        public class Handler : BaseTransactionScheduleHandler<Query, TransactionScheduleDetailsDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionScheduleRepository transactionScheduleRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionScheduleRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<TransactionScheduleDetailsDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var transactionScheduleEntity = await TransactionScheduleRepository.GetByIdAsync(request.TransactionScheduleId);
                if (transactionScheduleEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionScheduleEntity.Id))
                {
                    throw new NotFoundException("Target Transaction Schedule was not found.");
                }

                return Mapper.Map<TransactionScheduleDetailsDto>(transactionScheduleEntity);
            }
        }

    }
}