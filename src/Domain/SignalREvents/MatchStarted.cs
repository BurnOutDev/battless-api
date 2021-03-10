namespace Domain.SignalREvents
{
    public class MatchStarted : SignalMessage
    {
        public MatchStarted(string receiverEmail, decimal startPrice, decimal threshold, string opponentName, long unixThreshold, long startUnix) : base(receiverEmail, nameof(MatchStarted))
        {
            StartPrice = startPrice;
            Threshold = threshold;
            OpponentName = opponentName;
            StartUnix = startUnix;
            UnixThreshold = unixThreshold;
        }

        public decimal StartPrice { get; set; }
        public decimal Threshold { get; set; }
        public long UnixThreshold { get; set; }
        public long StartUnix { get; set; }
        public string OpponentName { get; set; }
    }
}
