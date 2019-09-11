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
        public async Task BudgetAdded(Guid userId, BudgetDto newBudget)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("BudgetAdded", newBudget);
        }

        public async Task BudgetRemoved(Guid userId, int budgetId)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("BudgetRemoved", budgetId);
        }

        public async Task BudgetUpdated(Guid userId, BudgetDto updatedBudget)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("BudgetUpdated", updatedBudget);
        }

        public async Task CategoryAdded(Guid userId, BudgetCategoryDto newCategory)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("CategoryAdded", newCategory);
        }

        public async Task CategoryRemoved(Guid userId, int categoryId)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("CategoryRemoved", categoryId);
        }

        public async Task CategoryUpdated(Guid userId, BudgetCategoryDto updatedCategory)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("CategoryUpdated", updatedCategory);

        }
    }
}
