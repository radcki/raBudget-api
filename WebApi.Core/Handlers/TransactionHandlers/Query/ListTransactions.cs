using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.TransactionHandlers.Query
{
    public class ListTransactions
    {
        public class Query : IRequest<Response>, IHaveCustomMapping
        {
            public Query()
            {
                CategoryId = new List<int>();
                CreatedByUserId = new List<Guid>();
            }

            public int BudgetId { get; set; }
            public IEnumerable<int> CategoryId { get; set; }
            public IEnumerable<Guid> CreatedByUserId { get; set; }

            public DateTime? TransactionDateStart { get; set; }
            public DateTime? TransactionDateEnd { get; set; }

            public DateTime? CreationDateStart { get; set; }
            public DateTime? CreationDateEnd { get; set; }

            public int? LimitCategoryTypeResults { get; set; }
            public eBudgetCategoryType? CategoryType { get; set; }

            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Query, TransactionsFilterModel>()
                             .ForMember(filter => filter.CategoryIdFilter, opt => opt.MapFrom(dto => dto.CategoryId))
                             .ForMember(filter => filter.CreatedByUserIdFilter, opt => opt.MapFrom(dto => dto.CreatedByUserId))
                             .ForMember(filter => filter.CreationDateStartFilter, opt => opt.MapFrom(dto => dto.CreationDateStart))
                             .ForMember(filter => filter.CreationDateEndFilter, opt => opt.MapFrom(dto => dto.CreationDateEnd))
                             .ForMember(filter => filter.TransactionDateStartFilter, opt => opt.MapFrom(dto => dto.TransactionDateStart))
                             .ForMember(filter => filter.TransactionDateEndFilter, opt => opt.MapFrom(dto => dto.TransactionDateEnd));
            }
        }

        public class Response
        {
            public List<TransactionDto> Data;
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseTransactionHandler<Query, Response>
        {
            private readonly IBudgetRepository _budgetRepository;

            public Handler
            (ITransactionRepository transactionRepository,
             IBudgetRepository budgetRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(null, transactionRepository, mapper, authenticationProvider)
            {
                _budgetRepository = budgetRepository;
            }

            public override async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                if (!await _budgetRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, query.BudgetId))
                {
                    throw new AccessViolationException("Requested budget not available");
                }

                var filters = Mapper.Map<TransactionsFilterModel>(query);

                var transactions = await TransactionRepository.ListWithFilter(new Budget(query.BudgetId), filters);

                return new Response {Data = Mapper.Map<List<TransactionDto>>(transactions)};
            }
        }
    }
}