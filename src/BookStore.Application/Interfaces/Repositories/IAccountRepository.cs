using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> Login(Account account);
        Task<Account> GetAccountDetails(Account account);
        Task UpdateName(Account account);
        Task UpdatePhoneNumber(Account account);
        Task UpdateAddress(Account account);
        Task ChangePassword(int id, string newPassword);
    }
}
