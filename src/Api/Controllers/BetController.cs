using Api.Controllers;
using Application;
using CryptoVision.Api.Services;
using Domain.SignalREvents;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CryptoVision.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : BaseController
    {
        private readonly GameService gameService;

        public BetController(GameService gameService, IAccountService accountService)
        {
            this.gameService = gameService;
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
