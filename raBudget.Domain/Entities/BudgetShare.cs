using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Domain.Entities
{
    public class BudgetShare : BaseEntity<int>
    {
        public int BudgetId { get; set; }
        public Guid SharedWithUserId { get; set; }

        /*
         * Navigation properties
         */
        public virtual User SharedWithUser { get; set; }
        public virtual  Budget Budget { get; set; }
    }
}
