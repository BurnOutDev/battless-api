namespace Domain.Models.Accounts
{
    public class BalanceRequest
    {
        public decimal Amount { get; set; }
        public bool Decrease { get; set; }
    }
}