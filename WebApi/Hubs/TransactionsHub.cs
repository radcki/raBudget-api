using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IHubContext<TransactionsHub> _hubContext;

        public TransactionsNotifier(IHubContext<TransactionsHub> hubContext)
        {
            _hubContext = hubContext;
        }
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
    }
}
