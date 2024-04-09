using ApecMovieCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieServices.Domain.Models
{
    public class Movie : BaseModel
    {
        public string? Title { get; set; }
        public string? Category { get; set; } 
        public string? Description { get; set; }
        public string? Image { get; set;}
        public string? Director { get; set; } // Đạo diễn của bộ phim
    }
}
