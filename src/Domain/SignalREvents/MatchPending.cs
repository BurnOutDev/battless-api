using System;

namespace Domain.SignalREvents
{
    public class MatchPending : SignalMessage
    {
        public MatchPending(string receiverEmail, Guid gid, string opponentName) : base(receiverEmail, nameof(MatchPending))
        {
            GameId = gid;
            OpponentName = opponentName;
        }

        public Guid GameId { get; set; }

        public string OpponentName { get; set; }

        public override string ToString() => $"E: {ReceiverEmail} Opponent: {OpponentName}";
    }
}
