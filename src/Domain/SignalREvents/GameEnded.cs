namespace Domain.SignalREvents
{
    public class GameEnded : SignalMessage
    {
        public GameEnded(string receiverEmail, bool isWinner, bool isDraw) : base(receiverEmail, nameof(GameEnded))
        {
            Won = isWinner;
            Draw = isDraw;
        }

        public bool Won { get; set; }
        public bool Draw { get; set; }
    }
}
