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
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Features.Budget.Command
{
    public class RevokeBudgetAccess
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
                var findShareTask = _budgetShareRepository.ListWithFilter(new Domain.Entities.Budget(request.BudgetId),
                                                                          new BudgetShareFilterModel()
                                                                          {
                                                                              UserIdFilter = request.UserId
                                                                          });
                if (!await BudgetRepository.IsAccessibleToUser(request.BudgetId))
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

                return new Response() {Data = new Unit()};
            }
        }
    }
}