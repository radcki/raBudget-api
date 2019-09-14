using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Models.Dtos;

namespace WebApi.Hubs
{
    [Authorize]
    public class TransactionsHub : Hub
    {
    }

    public class TransactionsNotifier
    {
        #region Privates

        private readonly IHubContext<TransactionsHub> _hubContext;

        #endregion

        #region Constructors

        public TransactionsNotifier(IHubContext<TransactionsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        #endregion

        #region Methods

        public async Task TransactionAdded(Guid userId, TransactionDto newTransaction)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("TransactionAdded", newTransaction);
        }

        public async Task TransactionRemoved(Guid userId, int transactionId)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("TransactionRemoved", transactionId);
        }

        public async Task TransactionUpdated(Guid userId, TransactionDto updatedTransaction)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("TransactionUpdated", updatedTransaction);
        }

        #endregion
    }
}