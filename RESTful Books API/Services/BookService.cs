using AutoMapper;

namespace RESTful_Books_API.Services
{
    public class BookService : Profile
    {
        public BookService()
        {
            CreateMap<DTO.Book.ShortBookDto, Models.Book>().ReverseMap();
        }
    }
}
