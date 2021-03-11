using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class Game
    {
        public Guid Uid { get; set; } = Guid.NewGuid();

        public Account AccountWhoBetShort { get; set; }
        public Account AccountWhoBetLong { get; set; }

        public decimal Amount { get; set; }
        public long StartUnix { get; set; }

        public decimal StartPrice { get; set; }
        public decimal EndPrice { get; set; }

        public List<ResponseKlineStreamModel> KlineStreams { get; set; } = new List<ResponseKlineStreamModel>();
    }
}
