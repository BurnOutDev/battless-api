namespace Domain.SignalREvents
{
    public class BalanceUpdated : SignalMessage
    {
        public BalanceUpdated(string receiverEmail, decimal amount) : base(receiverEmail, nameof(BalanceUpdated))
        {
            Amount = amount;
        }

        public decimal Amount { get; set; }
    }
}
