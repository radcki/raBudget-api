namespace WebApi.Dtos
{
    public class TransactionsTransferDto
    {
        public int BudgetId { get; set; }
        public int SourceCategory { get; set; }
        public int TargetCategory { get; set; }
    }
}