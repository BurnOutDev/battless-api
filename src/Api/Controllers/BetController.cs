using Api.Controllers;
using Application;
using CryptoVision.Api.Services;
using Domain.Entities;
using Domain.SignalREvents;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Persistence;
using System.Collections.Generic;
using System.Linq;

namespace CryptoVision.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : BaseController
    {
        private readonly GameService gameService;
        private readonly MongoDbRepository<Game> gameRepository;

        public BetController(GameService gameService, IAccountService accountService, MongoDbRepository<Game> gameRepository)
        {
            this.gameService = gameService;
            this.gameRepository = gameRepository;
        }

        [HttpPost]
        public IActionResult AddBet(BetPlacement request)
        {
            gameService.AddBet(new BetModel
            {
                Amount = request.Amount,
                Long = request.IsRiseOrFall,
                Short = !request.IsRiseOrFall,
                User = new Player
                {
                    Email = Account.Email,
                    Name = $"{Account.FirstName} {Account.LastName}"
                }
            });

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult GetTableData()
        {
            var tableData = new TableData();

            var longMatches = gameRepository.Collection.Find(x => x.AccountWhoBetLong.Email == Account.Email).ToList();
            var shortMatches = gameRepository.Collection.Find(x => x.AccountWhoBetShort.Email == Account.Email).ToList();

            tableData.OrderHistory = new List<OrderHistoryItem>();

            var longHistoryItems = longMatches.Select(ended =>
            {
                var delta = ended.KlineStreams.LastOrDefault().KlineItems.ClosePrice - ended.KlineStreams.FirstOrDefault().KlineItems.ClosePrice;

                return new OrderHistoryItem
                {
                    Amount = ended.Amount,
                    BattleId = ended.Id.ToString().Substring(0, 6),
                    Bet = "LONG",
                    Currency = ended.Currency,
                    EndPrice = ended.KlineStreams.LastOrDefault().KlineItems.ClosePrice,
                    StartPrice = ended.KlineStreams.FirstOrDefault().KlineItems.ClosePrice,
                    Profit = delta > 0 ? ended.Amount : 0
                };
            });

            var shortHistoryItems = shortMatches.Select(ended =>
            {
                var delta = ended.KlineStreams.LastOrDefault().KlineItems.ClosePrice - ended.KlineStreams.FirstOrDefault().KlineItems.ClosePrice;

                return new OrderHistoryItem
                {
                    Amount = ended.Amount,
                    BattleId = ended.Id.ToString().Substring(0, 6),
                    Bet = "SHORT",
                    Currency = ended.Currency,
                    EndPrice = ended.KlineStreams.LastOrDefault().KlineItems.ClosePrice,
                    StartPrice = ended.KlineStreams.FirstOrDefault().KlineItems.ClosePrice,
                    Profit = delta < 0 ? ended.Amount : 0
                };
            });

            tableData.OrderHistory.AddRange(longHistoryItems);
            tableData.OrderHistory.AddRange(shortHistoryItems);

            tableData.WinningStreaks = new List<WinningStreakItem>();

            return Ok(tableData);
        }

        public class TableData
        {
            public List<OrderHistoryItem> OrderHistory { get; set; }
            public List<WinningStreakItem> WinningStreaks { get; set; }
        }

        public class OrderHistoryItem
        {
            public string BattleId { get; set; }
            public string Currency { get; set; }
            public string Bet { get; set; }
            public decimal Amount { get; set; }
            public decimal StartPrice { get; set; }
            public decimal EndPrice { get; set; }
            public decimal Profit { get; set; }
        }

        public class WinningStreakItem
        {
            public int Ranking { get; set; }
            public string Username { get; set; }
            public int WinningStreak { get; set; }
        }

        //[HttpGet("[action]")]
        //public IActionResult GetGame()
        //{
        //    string state;

        //    var matched = gameService.Matched.FirstOrDefault(x => x.AccountWhoBetLong.Email == Account.Email || x.AccountWhoBetShort.Email == Account.Email);
        //    var pending = gameService.PendingMatched.FirstOrDefault(x => x.AccountWhoBetLong.Email == Account.Email || x.AccountWhoBetShort.Email == Account.Email);
        //    var ended = gameService.EndedMatches.FirstOrDefault(x => x.AccountWhoBetLong.Email == Account.Email || x.AccountWhoBetShort.Email == Account.Email);
        //    var unmatchedLongs = gameService.UnmatchedLongBets.FirstOrDefault(x => x.Player.Email == Account.Email);
        //    var unmatchedShorts = gameService.UnmatchedShortBets.FirstOrDefault(x => x.Player.Email == Account.Email);

        //    if (matched != null)
        //    {
        //        state = GameState.Matched.ToString();
        //        return Ok(new MatchStarted(Account.Email, matched.KlineStreams.FirstOrDefault().KlineItems.ClosePrice, )
        //        {
        //            Name = nameof(MatchStarted),
        //            ReceiverEmail = Account.Email,
        //            OpponentName = matched.AccountWhoBetLong.Email == Account.Email ? matched.AccountWhoBetShort.Name : matched.AccountWhoBetLong.Name,
        //            StartPrice = matched.KlineStreams.FirstOrDefault().KlineItems.ClosePrice,
        //            StartUnix = matched.KlineStreams.FirstOrDefault().EventTime,
        //            Threshold = gameService.Threshold,
        //            UnixThreshold = gameService.ThresholdUnixTime
        //        });
        //    }

        //    if (pending != null)
        //    {
        //        state = GameState.Pending.ToString();
        //        return Ok(new
        //        {
        //            Game = pending,
        //            State = state
        //        });
        //    }

        //    if (ended != null)
        //    {
        //        state = GameState.Ended.ToString();
        //        return Ok(new
        //        {
        //            Game = ended,
        //            State = state
        //        });
        //    }

        //    if (unmatchedLongs != null)
        //    {
        //        state = GameState.Unmatched.ToString();
        //        return Ok(new
        //        {
        //            Game = unmatchedLongs,
        //            State = state
        //        });
        //    }

        //    if (unmatchedShorts != null)
        //    {
        //        state = GameState.Unmatched.ToString();
        //        return Ok(new
        //        {
        //            Game = unmatchedShorts,
        //            State = state
        //        });
        //    }

        //    return Ok(new
        //    {
        //        State = GameState.NotFound.ToString()
        //    });
        //}
    }

    public enum GameState
    {
        Unmatched,
        Pending,
        Matched,
        Ended,
        NotFound
    }

    public class BetPlacement
    {
        public decimal Amount { get; set; }
        public bool IsRiseOrFall { get; set; }
    }
}
