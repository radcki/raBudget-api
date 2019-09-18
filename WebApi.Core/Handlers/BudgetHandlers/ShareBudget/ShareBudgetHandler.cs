﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.ShareBudget
{
    public class ShareBudgetHandler : IRequestHandler<ShareBudgetRequest, ShareBudgetResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public ShareBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<ShareBudgetResponse> Handle(ShareBudgetRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}