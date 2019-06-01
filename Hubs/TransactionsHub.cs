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
        public async Task TransactionAdded(string userName, TransactionDto newTransaction)
        {
            await _hubContext.Clients.User(userName).SendAsync("TransactionAdded", newTransaction);
        }

        public async Task TransactionRemoved(string userName, int transactionId)
        {
            await _hubContext.Clients.User(userName).SendAsync("TransactionRemoved", transactionId);
        }

        public async Task TransactionUpdated(string userName, TransactionDto updatedTransaction)
        {
            await _hubContext.Clients.User(userName).SendAsync("TransactionUpdated", updatedTransaction);

        }
    }
}
