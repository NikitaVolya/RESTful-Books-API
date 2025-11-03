using AutoMapper;
using RESTful_Books_API.Models;
using RESTful_Books_API.DTO.Loan;


namespace RESTful_Books_API.Profiles
{
    public class LoanProfile : Profile
    {
        public LoanProfile() {

            CreateMap<Loan, ShortLoanDto>()
                .ForMember(dest => dest.UserEmail, src => src.MapFrom(opt => opt.User.Email));

            CreateMap<UpdateLoanDto, Loan>().ReverseMap();

            CreateMap<Loan, DetailsLoanDto>()
                .ForMember(dest => dest.User, src => src.MapFrom(opt => new DetailsLoanDto.UserData
                {
                    Id = opt.User.Id,
                    FullName = opt.User.FirstName + " " + opt.User.LastName,
                    FirstName = opt.User.FirstName,
                    LastName = opt.User.LastName,
                    Email = opt.User.Email,
                    MembershipDate = opt.User.MembershipDate
                }))
                .ForMember(dest => dest.Book, src => src.MapFrom(opt => new DetailsLoanDto.BookData
                {
                    Id = opt.Book.Id,
                    Title = opt.Book.Title,
                    Author = opt.Book.Author,
                    ISBN = opt.Book.ISBN
                }));
        }
    }
}
