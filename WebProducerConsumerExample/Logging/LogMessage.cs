namespace WebProducerConsumerExample.Logging
{
    public class LogMessage
    {
        public LogMessage(int level, string msg)
        {
            Level = level;
            Message = msg;
        }

        public int Level { get; set; }

        public string Message { get; set; }
    }
}