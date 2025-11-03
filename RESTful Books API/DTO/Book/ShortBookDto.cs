using System.ComponentModel.DataAnnotations;

namespace RESTful_Books_API.DTO.Book
{
    public class ShortBookDto
    {
        [MinLength(4)]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MinLength(4)]
        [MaxLength(50)]
        public string Author { get; set; } = null!;

        [Range(100000000, 9999999999999)]
        public int ISBN { get; set; }

        [Range(0, 300)]
        public int CopiesAvailable { get; set; }

    }
}
