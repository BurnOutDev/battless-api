using CryptoVision.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Application;
using Domain.SignalREvents;
using Domain.Entities;
using Domain.Models;

namespace CryptoVision.Api.Services
{
    public class GameService
    {
        public decimal Threshold = 150;
        public int ThresholdUnixTime = 60000;

        #region SortedDictionaries
        public SortedDictionary<long, Guid> TimeMatches { get; set; }
        public SortedDictionary<decimal, Guid> LongPriceMatches { get; set; }
        public SortedDictionary<decimal, Guid> ShortPriceMatches { get; set; }
        #endregion

        public HashSet<UnmatchedBet> UnmatchedLongBets { get; set; }
        public HashSet<UnmatchedBet> UnmatchedShortBets { get; set; }

        public List<Game> PendingMatched { get; set; }
        public List<Game> Matched { get; set; }

        public List<Game> EndedMatches { get; set; }

        public Dictionary<string, string> EmailConnectionId { get; set; }

        private readonly IHubContext<KlineHub> klineHub;
        private readonly IAccountService accountService;

        public GameService(IHubContext<KlineHub> klinehub, IAccountService accountService)
        {
            UnmatchedShortBets = new HashSet<UnmatchedBet>();
            UnmatchedLongBets = new HashSet<UnmatchedBet>();
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

        public void SendState(string email, decimal currentBalance)
        {
            string state;

            var matched = Matched.FirstOrDefault(x => x.AccountWhoBetLong.Email == email || x.AccountWhoBetShort.Email == email);
            var pending = PendingMatched.FirstOrDefault(x => x.AccountWhoBetLong.Email == email || x.AccountWhoBetShort.Email == email);
            var ended = EndedMatches.FirstOrDefault(x => x.AccountWhoBetLong.Email == email || x.AccountWhoBetShort.Email == email);
            var unmatchedLongs = UnmatchedLongBets.FirstOrDefault(x => x.Player.Email == email);
            var unmatchedShorts = UnmatchedShortBets.FirstOrDefault(x => x.Player.Email == email);

            if (matched != null)
            {
                state = GameState.Matched.ToString();
                var opponentName = matched.AccountWhoBetLong.Email == email ? matched.AccountWhoBetShort.Name : matched.AccountWhoBetLong.Name;

                var time = matched.KlineStreams.LastOrDefault().EventTime - matched.KlineStreams.FirstOrDefault().EventTime;

                SendMessage(new MatchStarted(email, matched.KlineStreams.FirstOrDefault().KlineItems.ClosePrice, Threshold, opponentName, ThresholdUnixTime, matched.KlineStreams.FirstOrDefault().EventTime, matched.Amount, time));
            }
            else if (pending != null)
            {
                state = GameState.Pending.ToString();
                var opponentName = pending.AccountWhoBetLong.Email == email ? pending.AccountWhoBetShort.Name : pending.AccountWhoBetLong.Name;

                SendMessage(new MatchPending(email, pending.Uid, opponentName));
            }
            else if (ended != null)
            {
                var delta = ended.KlineStreams.LastOrDefault().KlineItems.ClosePrice - ended.KlineStreams.FirstOrDefault().KlineItems.ClosePrice;

                if (delta < 0)
                {
                    SendMessage(new GameEnded(email, ended.AccountWhoBetShort.Email == email, false));
                }
                else
                {
                    SendMessage(new GameEnded(email, ended.AccountWhoBetLong.Email == email, false));
                }
            }
            else if (unmatchedLongs != null)
            {
                SendMessage(new BetPlaced(email, unmatchedLongs.Amount, true, false, currentBalance));
            }
            else if (unmatchedShorts != null)
            {
                SendMessage(new BetPlaced(email, unmatchedShorts.Amount, true, false, currentBalance));
            }
            else
            {
                SendMessage(new SignalMessage(email, "CLEAR"));
            }

            //return Ok(new
            //{
            //    State = GameState.NotFound.ToString()
            //});
        }

        private void MatchingTimer()
        {
            while (true)
            {
                UnmatchedLongBets.OrderBy(bet => bet.Amount).ToList().ForEach(x =>
                {
                    //Added email comparison to avoid matching to self
                    var sb = UnmatchedShortBets.FirstOrDefault(e => e.Amount == x.Amount && x.Player.Email != e.Player.Email);

                    if (sb != null)
                    {
                        var g = new Game
                        {
                            Amount = x.Amount,
                            AccountWhoBetShort = sb.Player,
                            AccountWhoBetLong = x.Player
                        };

                        UnmatchedLongBets.Remove(x);
                        UnmatchedShortBets.Remove(sb);

                        PendingMatched.Add(g);

                        SendMessage(new MatchPending(x.Player.Email, g.Uid, sb.Player.Name));
                        SendMessage(new MatchPending(sb.Player.Email, g.Uid, x.Player.Name));
                    }
                });

                Thread.Sleep(500);
            }
        }

        private int GetAccountId(string email) => accountService.GetAll().FirstOrDefault(x => x.Email == email).Id;

        public void AddBet(BetModel model)
        {
            var account = accountService.GetByEmail(model.User.Email);

            var bet = new UnmatchedBet
            {
                Amount = model.Amount,
                Type = model.Long ? BetType.Long : BetType.Short,
                Player = account
            };

            if (model.Long)
            {
                UnmatchedLongBets.Add(bet);
            }
            else if (model.Short)
            {
                UnmatchedShortBets.Add(bet);
            }

            var balance = accountService.UpdateBalance(account.Id, new Domain.Models.Accounts.BalanceRequest
            {
                Amount = model.Amount,
                Decrease = true
            });

            SendMessage(new BalanceUpdated(model.User.Email, balance.Amount));

            SendMessage(new BetPlaced(model.User.Email, model.Amount, model.Long, model.Short, balance.Amount));
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

                    SendMessage(new MatchStarted(x.AccountWhoBetLong.Email, closePrice, Threshold, x.AccountWhoBetShort.Name, ThresholdUnixTime, unixDate, x.Amount, data.EventTime));
                    SendMessage(new MatchStarted(x.AccountWhoBetShort.Email, closePrice, Threshold, x.AccountWhoBetLong.Name, ThresholdUnixTime, unixDate, x.Amount, data.EventTime));
                });
                PendingMatched.RemoveAll(x => Matched.Select(y => y.Uid).Contains(x.Uid));
            }

            Matched.ForEach(x =>
            {
                x.KlineStreams.Add(data);
                SendMessage(new PriceEvent(x.AccountWhoBetLong.Email, closePrice, unixDate));
                SendMessage(new PriceEvent(x.AccountWhoBetShort.Email, closePrice, unixDate));
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
                balance = accountService.UpdateBalance(GetAccountId(g.AccountWhoBetShort.Email), new Domain.Models.Accounts.BalanceRequest
                {
                    Amount = g.Amount * 2
                }).Amount;

                SendMessage(new BalanceUpdated(g.AccountWhoBetShort.Email, balance));

                SendMessage(new GameEnded(g.AccountWhoBetShort.Email, true, false));
                SendMessage(new GameEnded(g.AccountWhoBetLong.Email, false, false));
            }
            else
            {
                balance = accountService.UpdateBalance(GetAccountId(g.AccountWhoBetLong.Email), new Domain.Models.Accounts.BalanceRequest
                {
                    Amount = g.Amount * 2
                }).Amount;

                SendMessage(new BalanceUpdated(g.AccountWhoBetLong.Email, balance));

                SendMessage(new GameEnded(g.AccountWhoBetShort.Email, false, false));
                SendMessage(new GameEnded(g.AccountWhoBetLong.Email, true, false));
            }
        }

        public void SendMessage(SignalMessage message)
        {
            klineHub.Clients.Client(EmailConnectionId[message.ReceiverEmail]).SendAsync(message.Name, message);
            Console.WriteLine($"{message.Name}: {message}");
        }

        public void SendError(SignalMessage message)
        {
            klineHub.Clients.Client(EmailConnectionId[message.ReceiverEmail]).SendAsync(message.Name, message);
            Console.WriteLine($"{message.Name}: {message}");
        }
    }

    public enum BetType
    {
        Long = 1,
        Short = -1
    }

    public enum GameState
    {
        Unmatched,
        Pending,
        Matched,
        Ended,
        NotFound
    }

    public class UnmatchedBet
    {
        public decimal Amount { get; set; }
        public Account Player { get; set; }
        public BetType Type { get; set; }
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
