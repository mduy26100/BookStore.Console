using BookStore.App.Areas;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Application.Mappings;
using BookStore.Application.Services;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    //DI
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<IAccountService, AccountService>();
                    services.AddScoped<ICategoryService, CategoryService>();
                    services.AddScoped<IBookService, BookService>();
                    services.AddScoped<IShoppingCartService, ShoppingCartService>();
                    services.AddScoped<IOrderService, OrderService>();

                    //Mapper
                    services.AddAutoMapper(typeof(MappingProfile));

                    services.AddTransient<App>();
                })
                .Build();

var app = host.Services.GetRequiredService<App>();
await app.Run();