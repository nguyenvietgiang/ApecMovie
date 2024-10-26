using System.ComponentModel.DataAnnotations;

namespace BlogServices.Models.Entity
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // ID kiểu GUID

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; }

        public int ViewCount { get; set; } = 0; // Lượt view mặc định là 0

        public bool IsActive { get; set; } = true; // Trạng thái mặc định là true
    }
}
