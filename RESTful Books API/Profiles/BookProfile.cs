using AutoMapper;

namespace RESTful_Books_API.Profiles
{
    public class BookProfile : Profile
    {

        public BookProfile() {

            CreateMap<Models.BookModel, DTO.Book.ShortBookDto>().ReverseMap();

            CreateMap<Models.BookModel, DTO.Book.DetailsBookDto>()
                .ForMember(dest => dest.CurrentLoans, opt =>
                {
                    opt.MapFrom(src => src.Loans.Where(l => l.ReturnDate == null).Select(loan => new DTO.Book.DetailsBookDto.LoanData
                    {
                        Id = loan.Id,
                        UserId = loan.UserId,
                        UserFullName = $"{loan.User.FirstName} {loan.User.LastName}",
                        UserFirstName = loan.User.FirstName,
                        UserLastName = loan.User.LastName,
                        UserEmail = loan.User.Email,
                        LoanDate = loan.LoanDate,
                        ReturnDate = loan.ReturnDate
                    }).ToList());
                })
                .ForMember(dest => dest.PastLoans, opt =>
                {
                    opt.MapFrom(src => src.Loans.Where(l => l.ReturnDate != null).Select(loan => new DTO.Book.DetailsBookDto.LoanData
                    {
                        Id = loan.Id,
                        UserId = loan.UserId,
                        UserFullName = $"{loan.User.FirstName} {loan.User.LastName}",
                        UserFirstName = loan.User.FirstName,
                        UserLastName = loan.User.LastName,
                        UserEmail = loan.User.Email,
                        LoanDate = loan.LoanDate,
                        ReturnDate = loan.ReturnDate
                    }).ToList());
                })
                .ForMember(dest => dest.CopiesInStock, opt => { 
                    opt.MapFrom(src => src.CopiesAvailable - src.Loans.Where(l => l.ReturnDate == null).Count());
                });

        }
    }
}
