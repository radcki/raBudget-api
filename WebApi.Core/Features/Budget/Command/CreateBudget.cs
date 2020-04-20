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
using raBudget.Core.Features.Allocation.Command;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.Budget.Command
{
    public class CreateBudget
    {
        public class Command : IRequest<Response>
        {
            public string Name { get; set; }
            public CurrencyDto Currency { get; set; }
            public DateTime StartingDate { get; set; }

            public IEnumerable<BudgetCategoryDto> BudgetCategories { get; set; }

            public Command()
            {
                BudgetCategories = new List<BudgetCategoryDto>();
            }
        }


        public class CurrencyDto
        {
            public eCurrency CurrencyCode { get; set; }
            public string Code { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Command, Domain.Entities.Budget>()
                             .ForMember(entity => entity.CurrentFunds, opt => opt.Ignore())
                             .ForMember(entity => entity.OwnedByUser, opt => opt.Ignore())
                             .ForMember(entity => entity.CurrencyCode, opt => opt.MapFrom(dto => dto.Currency.CurrencyCode))
                             .ForMember(entity => entity.BudgetCategories, opt => opt.MapFrom(dto => dto.BudgetCategories));
            }
        }

        public class Response : BaseResponse<BudgetDto>
        {
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Currency).NotEmpty();
                RuleFor(x => x.StartingDate).NotEmpty();
            }
        }
        public class Notification : INotification
        {
            public int BudgetId { get; set; }
        }
        public class Handler : BaseBudgetHandler<Command, Response>
        {
            private readonly IMediator _mediator;

            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider, IMediator mediator) : base(repository, mapper, authenticationProvider)
            {
                _mediator = mediator;
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                var budgetEntity = Mapper.Map<Domain.Entities.Budget>(command);
                budgetEntity.OwnedByUserId = AuthenticationProvider.User.UserId;
                var savedBudget = await BudgetRepository.AddAsync(budgetEntity);

                var addedRows = await BudgetRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(budgetEntity), budgetEntity);
                }

                var dto = Mapper.Map<BudgetDto>(savedBudget);
                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = dto.BudgetId,
                                      }, cancellationToken);

                return new Response() {Data = dto};
            }
        }
    }
}