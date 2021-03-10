namespace Domain.Entities
{
    public class ResponseKlineStreamModel : BaseEntity
    {
        public string EventType { get; set; }
        public long EventTime { get; set; }
        public string Symbol { get; set; }
        public KlineItems KlineItems { get; set; }
    }
}
