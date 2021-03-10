using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ResponseKlineModel
    {
        public long OpenTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public long CloseTime { get; set; }
        public decimal QuoteAssetsValue { get; set; }
        public int Trades { get; set; }
        public decimal BuyBaseAsset { get; set; }
        public decimal BuyQuoteAsset { get; set; }
        public decimal Ignore { get; set; }
    }
}
