using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWSLab03.Models
{
    public class Episode
    {
        [Key]
        public int EpisodeID { get; set; }

        [Required]
        [ForeignKey("Podcast")]
        public int PodcastID { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Range(1, 1000)]
        public int Duration { get; set; } = 1;

        public int PlayCount { get; set; } = 0;

        public string? AudioFileURL { get; set; }

        public virtual Podcast? Podcast { get; set; }
    }
}
