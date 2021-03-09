using Api.Controllers;
using Application;
using CryptoVision.Api.Services;
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
        //    //var matched = gameService.Matched.FirstOrDefault(x => x.PlayerWhoBetLong.Email == Account.Email || x.PlayerWhoBetShort.Email == Account.Email);
        //    //var pending = gameService.PendingMatched.FirstOrDefault(x => x.PlayerWhoBetLong.Email == Account.Email || x.PlayerWhoBetShort.Email == Account.Email);
        //    //var ended = gameService.EndedMatches.FirstOrDefault(x => x.PlayerWhoBetLong.Email == Account.Email || x.PlayerWhoBetShort.Email == Account.Email);
        //    //var unmatchedLongs = gameService.UnmatchedLongBets.FirstOrDefault(x => x.User.Email == Account.Email);
        //    //var unmatchedShorts = gameService.UnmatchedShortBets.FirstOrDefault(x => x.User.Email == Account.Email);

        //    //if (matched != null)
        //    //{

        //    //}
        //}
    }

    public class BetPlacement
    {
        public decimal Amount { get; set; }
        public bool IsRiseOrFall { get; set; }
    }
}
