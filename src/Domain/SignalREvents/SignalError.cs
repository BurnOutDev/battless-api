using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SignalREvents
{

    public class SignalError : SignalMessage
    {
        public SignalError(string receiverEmail, string method, string message) : base(receiverEmail, method)
        {
            Message = message;
        }

        public SignalError(string receiverEmail, string message) : base(receiverEmail, nameof(SignalError))
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
