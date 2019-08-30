namespace API.Areas.Alive.Models
{
    /// <summary>
    /// A message of a notification
    /// </summary>
    public class NotificationMessage
    {
        /// <value>The id of the notification</value>
        public string id { get; set; }
        
        /// <value>The text of the notification</value>
        public string message { get; set; }
    }
}
