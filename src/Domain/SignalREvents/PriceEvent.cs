namespace Domain.SignalREvents
{
    public class PriceEvent : SignalMessage
    {
        public PriceEvent(string receiverEmail, decimal currentPrice, long currentUnix) : base(receiverEmail, nameof(PriceEvent))
        {
            CurrentPrice = currentPrice;
            CurrentUnix = currentUnix;
        }

        public decimal CurrentPrice { get; set; }
        public long CurrentUnix { get; set; }
    }
}
