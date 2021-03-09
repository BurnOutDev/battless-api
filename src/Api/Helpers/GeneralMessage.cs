using System.Collections.Generic;

namespace CryptoVision.Api.Models
{

    public class GeneralMessage<T>
    {
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public T Message { get; set; }
    }
}
