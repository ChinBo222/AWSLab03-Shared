using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWSLab03.Models
{
    public class Podcast
    {
        [Key]
        public int PodcastID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Description { get; set; }

        // Assigned automatically in controller
        public string? CreatorID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Nullable collection prevents ModelState from requiring Episodes on create
        public virtual ICollection<Episode>? Episodes { get; set; }
    }
}
