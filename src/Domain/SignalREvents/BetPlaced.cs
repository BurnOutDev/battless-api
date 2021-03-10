namespace Domain.SignalREvents
{
    public class BetPlaced : SignalMessage
    {
        public BetPlaced(string receiverEmail, decimal amount, bool @long, bool @short, decimal remainingBalance) : base(receiverEmail, nameof(BetPlaced))
        {
            Amount = amount;
            Long = @long;
            Short = @short;
            RemainingBalance = remainingBalance;
        }

        public decimal Amount { get; set; }
        public bool Long { get; set; }
        public bool Short { get; set; }
        public decimal RemainingBalance { get; set; }

        public override string ToString()
        {
            var bet = Long ? nameof(Long) : nameof(Short);

            return $"E: {ReceiverEmail} ${Amount} {bet}";
        }
    }
}
