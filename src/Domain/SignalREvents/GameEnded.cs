namespace Domain.SignalREvents
{
    public class GameEnded : SignalMessage
    {
        public GameEnded(string receiverEmail, bool isWinner, bool isDraw, decimal amount, bool isRiseOrFall, decimal startPrice, decimal closePrice) : base(receiverEmail, nameof(GameEnded))
        {
            Won = isWinner;
            Draw = isDraw;
            Amount = amount;
            IsRiseOrFall = isRiseOrFall;
            StartPrice = startPrice;
            ClosePrice = closePrice;
        }

        public bool Won { get; set; }
        public bool Draw { get; set; }

        public decimal StartPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public bool IsRiseOrFall { get; set; }
        public decimal Amount { get; set; }

    }
}
