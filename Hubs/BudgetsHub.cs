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
    public class BudgetsHub : Hub
    {

    }

    public class BudgetsNotifier
    {
        private readonly IHubContext<BudgetsHub> _hubContext;

        public BudgetsNotifier(IHubContext<BudgetsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task BudgetAdded(string userId, BudgetDto newBudget)
        {
            await _hubContext.Clients.User(userId).SendAsync("BudgetAdded", newBudget);
        }

        public async Task BudgetRemoved(string userId, int budgetId)
        {
            await _hubContext.Clients.User(userId).SendAsync("BudgetRemoved", budgetId);
        }

        public async Task BudgetUpdated(string userId, BudgetDto updatedBudget)
        {
            await _hubContext.Clients.User(userId).SendAsync("BudgetUpdated", updatedBudget);
        }

        public async Task CategoryAdded(string userName, BudgetCategoryDto newCategory)
        {
            await _hubContext.Clients.User(userName).SendAsync("CategoryAdded", newCategory);
        }

        public async Task CategoryRemoved(string userName, int categoryId)
        {
            await _hubContext.Clients.User(userName).SendAsync("CategoryRemoved", categoryId);
        }

        public async Task CategoryUpdated(string userName, BudgetCategoryDto updatedCategory)
        {
            await _hubContext.Clients.User(userName).SendAsync("CategoryUpdated", updatedCategory);

        }
    }
}
