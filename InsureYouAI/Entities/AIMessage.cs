namespace InsureYouAI.Entities
{
    public class AIMessage
    {
        public int AIMessageId { get; set; }
        public string MessageDetail { get; set; }
        public string ReceiveMail { get; set; }
        public string ReceiveNameSurname { get; set; }
        public DateTime SendDate { get; set; }
    }
}
