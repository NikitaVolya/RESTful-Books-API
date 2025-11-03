using AutoMapper;
using RESTful_Books_API.DTO.User;


namespace RESTful_Books_API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<ShortUserDto, Models.UserModel>().ReverseMap();

            CreateMap<CreateUserDto, Models.UserModel>()
                .ForMember(dest => dest.PasswordHash, opt =>
                {
                    opt.Ignore();
                });

            CreateMap<Models.UserModel, DetailsUserDto>()
                .ForMember(dest => dest.CurrentLoans, opt =>
                {
                    opt.MapFrom(src => src.Loans.Where(l => l.ReturnDate == null).Select(loan => new DetailsUserDto.LoanData
                    {
                        Id = loan.Id,
                        BookId = loan.BookId,
                        BookTitle = loan.Book.Title,
                        LoanDate = loan.LoanDate,
                        ReturnDate = loan.ReturnDate
                    }).ToList());
                })
                .ForMember(dest => dest.PastLoans, opt =>
                {
                    opt.MapFrom(src => src.Loans.Where(l => l.ReturnDate != null).Select(loan => new DetailsUserDto.LoanData
                    {
                        Id = loan.Id,
                        BookId = loan.BookId,
                        BookTitle = loan.Book.Title,
                        LoanDate = loan.LoanDate,
                        ReturnDate = loan.ReturnDate
                    }).ToList());
                });
        }
    }
}
