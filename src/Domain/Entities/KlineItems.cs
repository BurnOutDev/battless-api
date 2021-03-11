namespace Domain.Entities
{
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
