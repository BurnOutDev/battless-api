using CryptoVision.Api.Hubs;
using CryptoVision.Api.Models;
using Microsoft.AspNetCore.SignalR;
using SignalREvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using static Game;
using Api.Hubs;
using CryptoVision.Api.Services;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Application;

namespace CryptoVision.Api.Services
{
    public class GameService
    {
        private decimal Threshold = 150;
        private int ThresholdUnixTime = 60000;

        #region SortedDictionaries
        public SortedDictionary<long, Guid> TimeMatches { get; set; }
        public SortedDictionary<decimal, Guid> LongPriceMatches { get; set; }
        public SortedDictionary<decimal, Guid> ShortPriceMatches { get; set; }
        #endregion

        public HashSet<BetModel> UnmatchedLongBets { get; set; }
        public HashSet<BetModel> UnmatchedShortBets { get; set; }

        public List<Game> PendingMatched { get; set; }
        public List<Game> Matched { get; set; }

        public List<Game> EndedMatches { get; set; }

        public Dictionary<string, string> EmailConnectionId { get; set; }

        private readonly IHubContext<KlineHub> klineHub;
        private readonly IAccountService accountService;

        public GameService(IHubContext<KlineHub> klinehub, IAccountService accountService)
        {
            UnmatchedShortBets = new HashSet<BetModel>();
            UnmatchedLongBets = new HashSet<BetModel>();
            TimeMatches = new SortedDictionary<long, Guid>();
            LongPriceMatches = new SortedDictionary<decimal, Guid>();
            ShortPriceMatches = new SortedDictionary<decimal, Guid>();
            PendingMatched = new List<Game>();
            Matched = new List<Game>();
            EndedMatches = new List<Game>();
            EmailConnectionId = new Dictionary<string, string>();
            this.accountService = accountService;

            Task.Run(MatchingTimer);

            klineHub = klinehub;
        }

        private void MatchingTimer()
        {
            while (true)
            {
                var mat = new List<Tuple<BetModel, BetModel, Game>>();

                UnmatchedLongBets.OrderBy(bet => bet.Amount).ToList().ForEach(x =>
                {
                    //Added email comparison to avoid matching to self
                    var sb = UnmatchedShortBets.FirstOrDefault(e => e.Amount == x.Amount && x.User.Email != e.User.Email);

                    if (sb != null)
                    {
                        var g = new Game
                        {
                            Amount = x.Amount,
                            PlayerWhoBetShort = sb.User,
                            PlayerWhoBetLong = x.User
                        };

                        UnmatchedLongBets.Remove(x);
                        UnmatchedShortBets.Remove(sb);

                        PendingMatched.Add(g);

                        SendMessage(new MatchPending(x.User, g.Uid, sb.User.Name));
                        SendMessage(new MatchPending(sb.User, g.Uid, x.User.Name));
                    }
                });

                Thread.Sleep(500);
            }
        }

        private int GetAccountId(string email) => accountService.GetAll().FirstOrDefault(x => x.Email == email).Id;

        public void AddBet(BetModel model)
        {
            if (model.Long)
            {
                UnmatchedLongBets.Add(model);
            }
            else if (model.Short)
            {
                UnmatchedShortBets.Add(model);
            }

            var balance = accountService.UpdateBalance(GetAccountId(model.User.Email), new Domain.Models.Accounts.BalanceRequest
            {
                Amount = model.Amount,
                Decrease = true
            });

            SendMessage(new BalanceUpdated(model.User, balance.Amount));

            SendMessage(new BetPlaced(model.User, model.Amount, model.Long, model.Short, balance.Amount));
        }

        public void PriceUpdated(ResponseKlineStreamModel data)
        {
            #region Match process
            var unixDate = data.EventTime;
            var closePrice = data.KlineItems.ClosePrice;

            if (PendingMatched.Count > 0)
            {
                //TODO Matched and PendingMatched can be filled with new values between steps
                //var g = PendingMatched.Select(x => x.Uid);

                var gids = PendingMatched.Select(x => x.Uid);

                Matched.AddRange(PendingMatched.Where(x => gids.Contains(x.Uid)));

                PendingMatched.Where(x => gids.Contains(x.Uid)).ToList().ForEach(x =>
                {
                    TimeMatches.Add(unixDate + ThresholdUnixTime, x.Uid);
                    LongPriceMatches.Add(closePrice + Threshold, x.Uid);
                    ShortPriceMatches.Add(closePrice - Threshold, x.Uid);

                    SendMessage(new MatchStarted(x.PlayerWhoBetLong, closePrice, Threshold, x.PlayerWhoBetShort.Name, ThresholdUnixTime, data.EventTime));
                    SendMessage(new MatchStarted(x.PlayerWhoBetShort, closePrice, Threshold, x.PlayerWhoBetLong.Name, ThresholdUnixTime, data.EventTime));
                });
                PendingMatched.RemoveAll(x => Matched.Select(y => y.Uid).Contains(x.Uid));
            }

            Matched.ForEach(x =>
            {
                x.KlineStreams.Add(data);
                SendMessage(new PriceEvent(x.PlayerWhoBetLong, closePrice, unixDate));
                SendMessage(new PriceEvent(x.PlayerWhoBetShort, closePrice, unixDate));
            });
            #endregion

            LongPriceMatches.Keys.ToList().ForEach(x =>
            {
                if (closePrice > x)
                    EndGame(LongPriceMatches[x]);
            });

            ShortPriceMatches.Keys.ToList().ForEach(x =>
            {
                if (closePrice < x)
                    EndGame(ShortPriceMatches[x]);
            });

            TimeMatches.Keys.ToList().ForEach(x =>
            {
                if (unixDate > x)
                    EndGame(TimeMatches[x]);
            });
        }

        public void EndGame(Guid gid)
        {
            var g = Matched.FirstOrDefault(q => q.Uid == gid);

            Matched.Remove(g);
            EndedMatches.Add(g);

            LongPriceMatches.Remove(LongPriceMatches.FirstOrDefault(m => m.Value == g.Uid).Key);
            ShortPriceMatches.Remove(ShortPriceMatches.FirstOrDefault(m => m.Value == g.Uid).Key);
            TimeMatches.Remove(TimeMatches.FirstOrDefault(m => m.Value == g.Uid).Key);

            var delta = g.KlineStreams.LastOrDefault().KlineItems.ClosePrice - g.KlineStreams.FirstOrDefault().KlineItems.ClosePrice;

            decimal balance = 0;

            if (delta < 0)
            {
                balance = accountService.UpdateBalance(GetAccountId(g.PlayerWhoBetShort.Email), new Domain.Models.Accounts.BalanceRequest
                {
                    Amount = g.Amount * 2
                }).Amount;

                SendMessage(new BalanceUpdated(g.PlayerWhoBetShort, balance));

                SendMessage(new GameEnded(g.PlayerWhoBetShort, true, false));
                SendMessage(new GameEnded(g.PlayerWhoBetLong, false, false));
            }
            else
            {
                balance = accountService.UpdateBalance(GetAccountId(g.PlayerWhoBetLong.Email), new Domain.Models.Accounts.BalanceRequest
                {
                    Amount = g.Amount * 2
                }).Amount;

                SendMessage(new BalanceUpdated(g.PlayerWhoBetLong, balance));

                SendMessage(new GameEnded(g.PlayerWhoBetShort, false, false));
                SendMessage(new GameEnded(g.PlayerWhoBetLong, true, false));
            }
        }

        public void SendMessage(SignalMessage message)
        {
            klineHub.Clients.Client(EmailConnectionId[message.Player.Email]).SendAsync(message.Name, message);
            Console.WriteLine($"{message.Name}: {message}");
        }

        public void SendError(SignalMessage message)
        {
            klineHub.Clients.Client(EmailConnectionId[message.Player.Email]).SendAsync(message.Name, message);
            Console.WriteLine($"{message.Name}: {message}");
        }
    }
}

namespace SignalREvents
{
    public class BetPlaced : SignalMessage
    {
        public BetPlaced(Player receiver, decimal amount, bool @long, bool @short, decimal remainingBalance) : base(receiver, nameof(BetPlaced))
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

            return $"E: {Player.Email} ${Amount} {bet}";
        }
    }

    public class BalanceUpdated : SignalMessage
    {
        public BalanceUpdated(Player receiver, decimal amount) : base(receiver, nameof(BalanceUpdated))
        {
            Amount = amount;
        }

        public decimal Amount { get; set; }
    }

    public class MatchPending : SignalMessage
    {
        public MatchPending(Player receiver, Guid gid, string opponentName) : base(receiver, nameof(MatchPending))
        {
            GameId = gid;
            OpponentName = opponentName;
        }

        public Guid GameId { get; set; }

        public string OpponentName { get; set; }

        public override string ToString() => $"E: {Player.Email} Opponent: {OpponentName}";
    }

    public class MatchStarted : SignalMessage
    {
        public MatchStarted(Player receiver, decimal startPrice, decimal threshold, string opponentName, long unixThreshold, long startUnix) : base(receiver, nameof(MatchStarted))
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

    public class GameEnded : SignalMessage
    {
        public GameEnded(Player receiver, bool isWinner, bool isDraw) : base(receiver, nameof(GameEnded))
        {
            Won = isWinner;
            Draw = isDraw;
        }

        public bool Won { get; set; }
        public bool Draw { get; set; }
    }

    public class PriceEvent : SignalMessage
    {
        public PriceEvent(Player receiver, decimal currentPrice, long currentUnix) : base(receiver, nameof(PriceEvent))
        {
            CurrentPrice = currentPrice;
            CurrentUnix = currentUnix;
        }

        public decimal CurrentPrice { get; set; }
        public long CurrentUnix { get; set; }
    }

    public class SignalMessage
    {
        public SignalMessage(Player receiver, string name)
        {
            Player = receiver;
            Name = name;
        }

        public Player Player { get; set; }
        public string Name { get; set; }
    }

    public class SignalError : SignalMessage
    {
        public SignalError(Player receiver, string method, string message) : base(receiver, method)
        {
            Message = message;
        }

        public SignalError(Player receiver, string message) : base(receiver, nameof(SignalError))
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}

public class BetModel
{
    public decimal Amount { get; set; }
    public Player User { get; set; }
    public bool Long { get; set; }
    public bool Short { get; set; }
}

public class Player
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public partial class Game
{
    public Guid Uid { get; set; } = Guid.NewGuid();

    public Player PlayerWhoBetShort { get; set; }
    public Player PlayerWhoBetLong { get; set; }

    public decimal Amount { get; set; }
    public long StartUnix { get; set; }

    public decimal StartPrice { get; set; }
    public decimal EndPrice { get; set; }

    public List<ResponseKlineStreamModel> KlineStreams { get; set; } = new List<ResponseKlineStreamModel>();
}