using AutoMapper;


namespace RESTful_Books_API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<DTO.ShortUserDto, Models.User>().ReverseMap();

            CreateMap<Models.User, DTO.DetailsUserDto>()
                .ForMember(dest => dest.Loans, opt =>
                {
                    opt.MapFrom(src => src.Loans.Select(loan => new DTO.DetailsUserDto.LoanData
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
