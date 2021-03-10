namespace Domain.SignalREvents
{
    public class SignalMessage
    {
        public SignalMessage(string receiverEmail, string name)
        {
            ReceiverEmail = receiverEmail;
            Name = name;
        }

        public string ReceiverEmail { get; set; }
        public string Name { get; set; }
    }
}
