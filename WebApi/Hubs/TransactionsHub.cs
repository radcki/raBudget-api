using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using raBudget.Core.Interfaces;
using WebApi.Models.Dtos;

namespace WebApi.Hubs
{
    [Authorize]
    public class TransactionsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }

    public enum eTransactionHubEvent
    {
        TransactionAdded,
        TransactionRemoved,
        TransactionUpdated
    }

    public class TransactionsNotifier
    {
        #region Privates

        private readonly IHubContext<TransactionsHub> _hubContext;
        private readonly IAuthenticationProvider _authenticationProvider;
        private string UserId => _authenticationProvider.User.UserId.ToString();

        #endregion

        #region Constructors

        public TransactionsNotifier(IHubContext<TransactionsHub> hubContext, IAuthenticationProvider authenticationProvider)
        {
            _hubContext = hubContext;
            _authenticationProvider = authenticationProvider;
        }

        #endregion

        #region Methods

        public async Task Send(eTransactionHubEvent eventType, INotification payload)
        {
            await _hubContext.Clients.User(UserId).SendAsync(eventType.ToString(), payload);
        }

        #endregion
    }
}