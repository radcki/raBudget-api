using System;
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
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;

namespace raBudget.Core.Features.Budget.Command
{
    public class ShareBudget
    {
        public class Request : IRequest<Response>, IHaveCustomMapping
        {
            public int BudgetId { get; set; }
            public Guid UserId { get; set; }

            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Request, BudgetShare>()
                             .ForMember(entity => entity.Budget, opt => opt.MapFrom(dto => dto.BudgetId))
                             .ForMember(entity => entity.SharedWithUserId, opt => opt.MapFrom(dto => dto.UserId));
            }
        }

        public class Response : BaseResponse<Unit>
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Request, Response>
        {
            private readonly IBudgetShareRepository _budgetShareRepository;

            public Handler(IBudgetRepository repository, IBudgetShareRepository budgetShareRepository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
                _budgetShareRepository = budgetShareRepository;
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var availableBudgets = await BudgetRepository.ListAvailableBudgets();
                var budgetToUpdate = availableBudgets.FirstOrDefault(x => x.Id != request.BudgetId);
                if (budgetToUpdate == null)
                {
                    throw new NotFoundException("Budget was not found");
                }

                if (budgetToUpdate.BudgetShares.Any(x => x.SharedWithUserId == request.UserId))
                {
                    throw new SaveFailureException("Budget share is already created", request);
                }

                var shareEntity = Mapper.Map<BudgetShare>(request);
                await _budgetShareRepository.AddAsync(shareEntity);
                await _budgetShareRepository.SaveChangesAsync(cancellationToken);

                return new Response() {Data = new Unit()};
            }
        }
    }
}