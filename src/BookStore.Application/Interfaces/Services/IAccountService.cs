using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccount();
        Task<AccountDto> GetDetailAccount(AccountDto accountDto);
        Task<bool> Login(AccountDto accountDto);
        Task RegisterAccount(AccountCreateDto accountCreate);
        Task UpdateNameAccount(AccountDto accountDto);
        Task UpdatePhoneAccount(AccountDto accountDto);
        Task UpdateAddressAccount(AccountDto accountDto);
        Task ChangePasswordAccount(int id, string newPassword);

        // New methods for enhanced business logic
        Task<bool> CheckUsernameExists(string username);
        Task<bool> VerifyPassword(AccountDto accountDto);
    }
}
