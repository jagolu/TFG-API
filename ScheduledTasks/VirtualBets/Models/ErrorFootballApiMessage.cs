namespace API.ScheduledTasks.VirtualBets.Models
{
    /// <summary>
    /// Error message from the football api
    /// </summary>
    public class ErrorFootballApiMessage
    {
        /// <value>The error message</value>
        public string message { get; set; }
        
        /// <value>The code of the error</value>
        public int errorCode { get; set; }
    }
}
