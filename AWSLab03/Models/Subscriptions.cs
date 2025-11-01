using System;
using System.ComponentModel.DataAnnotations;

namespace AWSLab03.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionID { get; set; }

        public int PodcastID { get; set; }
        public string UserID { get; set; }
        public DateTime SubscribedDate { get; set; }

        // Navigation property
        public virtual Podcast Podcast { get; set; }
    }
}
