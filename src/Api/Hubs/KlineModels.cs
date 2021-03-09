using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Hubs
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

    public class ResponseKlineStreamModel
    {
        public string EventType { get; set; }
        public long EventTime { get; set; }
        public string Symbol { get; set; }
        public KlineItems KlineItems { get; set; }
    }
    public class KlineItems
    {
        public long KlineStartTime { get; set; }
        public long KlineCloseTime { get; set; }
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public int FirstTradeId { get; set; }
        public int LastTradeId { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal BaseAssetVolume { get; set; }
        public int NumberOfTrades { get; set; }
        public bool IsThisKlineClosed { get; set; }
        public decimal QuoteAssetsVolume { get; set; }
        public decimal TakerBuyBaseAssetVolume { get; set; }
        public decimal TakerBuyQuoteAssetVolume { get; set; }
        public string Ignore { get; set; }
    }
}
