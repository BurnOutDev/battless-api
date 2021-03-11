namespace Domain.SignalREvents
{
    public class GameEnded : SignalMessage
    {
        public GameEnded(string receiverEmail, bool isWinner, bool isDraw, bool isRiseOrFall) : base(receiverEmail, nameof(GameEnded))
        {
            Won = isWinner;
            Draw = isDraw;
            IsRiseOrFall = isRiseOrFall;
        }

        public bool Won { get; set; }
        public bool Draw { get; set; }
        public bool IsRiseOrFall { get; set; }
    }
}
