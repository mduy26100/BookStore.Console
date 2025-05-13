using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Account
            CreateMap<Account, AccountDto>();
            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountCreateDto>();
            CreateMap<AccountCreateDto, Account>();

            // Book
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            // ShoppingCart
            CreateMap<ShoppingCart, CartItemDto>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));
            CreateMap<CartItemDto, ShoppingCart>()
                .ForMember(dest => dest.Book, opt => opt.Ignore());

            // Order
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.Reports, opt => opt.MapFrom(src => src.Reports));
            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.Reports, opt => opt.Ignore()); // Avoid circular references

            // OrderDetail
            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailDto, OrderDetail>();

            // Report
            CreateMap<Report, ReportDto>();
            CreateMap<ReportDto, Report>();
        }
    }
}
