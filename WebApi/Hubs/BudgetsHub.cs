﻿using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using raBudget.Core.Interfaces;
using WebApi.Models.Dtos;

namespace WebApi.Hubs
{
    [Authorize]
    public class BudgetsHub : Hub
    {
    }

    public enum eBudgetHubEvent
    {
        BudgetAdded,
        BudgetRemoved,
        BudgetUpdated,
        BudgetCategoryAdded,
        BudgetCategoryRemoved,
        BudgetCategoryUpdated,
        BudgetCategoryReordered
    }

    public class BudgetsNotifier
    {
        #region Privates

        private readonly IHubContext<BudgetsHub> _hubContext;
        private readonly IAuthenticationProvider _authenticationProvider;
        private string UserId => _authenticationProvider.User.UserId.ToString();

        #endregion

        #region Constructors

        public BudgetsNotifier(IHubContext<BudgetsHub> hubContext, IAuthenticationProvider authenticationProvider)
        {
            _hubContext = hubContext;
            _authenticationProvider = authenticationProvider;
        }

        #endregion

        #region Methods

        public async Task Send(eBudgetHubEvent eventType, INotification payload)
        {
            await _hubContext.Clients.User(UserId).SendAsync(eventType.ToString(), payload);
        }
        
        #endregion
    }
}