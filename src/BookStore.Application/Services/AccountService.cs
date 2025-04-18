using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task ChangePasswordAccount(int id, string newPassword)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid account ID");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty");

            try
            {
                await _unitOfWork.AccountRepository.ChangePassword(id, newPassword);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to change password: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccount()
        {
            try
            {
                var list = await _unitOfWork.AccountRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<AccountDto>>(list);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve accounts: {ex.Message}", ex);
            }
        }

        public async Task<AccountDto> GetDetailAccount(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            try
            {
                // First find the account by username since we might not have the ID after login
                var accounts = await _unitOfWork.AccountRepository.FindAsync(a =>
                    a.Username == accountDto.Username && a.Password == accountDto.Password);

                var account = accounts.FirstOrDefault();
                if (account == null)
                    throw new Exception("Account not found");

                return _mapper.Map<AccountDto>(account);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get account details: {ex.Message}", ex);
            }
        }

        public async Task<bool> Login(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            if (string.IsNullOrWhiteSpace(accountDto.Username) || string.IsNullOrWhiteSpace(accountDto.Password))
                throw new ArgumentException("Username and password are required");

            try
            {
                return await _unitOfWork.AccountRepository.Login(_mapper.Map<Account>(accountDto));
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}", ex);
            }
        }

        public async Task RegisterAccount(AccountCreateDto accountCreate)
        {
            if (accountCreate == null)
                throw new ArgumentNullException(nameof(accountCreate));

            if (string.IsNullOrWhiteSpace(accountCreate.Username) || string.IsNullOrWhiteSpace(accountCreate.Password))
                throw new ArgumentException("Username and password are required");

            try
            {
                // Check if username already exists
                var existingAccounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Username == accountCreate.Username);
                if (existingAccounts.Any())
                    throw new Exception("Username already exists");

                await _unitOfWork.AccountRepository.AddAsync(_mapper.Map<Account>(accountCreate));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}", ex);
            }
        }

        public async Task UpdateAddressAccount(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            if (accountDto.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            try
            {
                await _unitOfWork.AccountRepository.UpdateAddress(_mapper.Map<Account>(accountDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update address: {ex.Message}", ex);
            }
        }

        public async Task UpdateNameAccount(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            if (accountDto.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            if (string.IsNullOrWhiteSpace(accountDto.Name))
                throw new ArgumentException("Name cannot be empty");

            try
            {
                await _unitOfWork.AccountRepository.UpdateName(_mapper.Map<Account>(accountDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update name: {ex.Message}", ex);
            }
        }

        public async Task UpdatePhoneAccount(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            if (accountDto.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            try
            {
                await _unitOfWork.AccountRepository.UpdatePhoneNumber(_mapper.Map<Account>(accountDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update phone number: {ex.Message}", ex);
            }
        }

        // New methods for enhanced business logic

        /// <summary>
        /// Checks if a username already exists
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if username exists, false otherwise</returns>
        public async Task<bool> CheckUsernameExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            try
            {
                var existingAccounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Username == username);
                return existingAccounts.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check username: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifies if the provided password matches the account's password
        /// </summary>
        /// <param name="accountDto">Account with username and password to verify</param>
        /// <returns>True if password is correct, false otherwise</returns>
        public async Task<bool> VerifyPassword(AccountDto accountDto)
        {
            if (accountDto == null)
                throw new ArgumentNullException(nameof(accountDto));

            if (string.IsNullOrWhiteSpace(accountDto.Password))
                throw new ArgumentException("Password cannot be empty");

            try
            {
                // If we have the account ID, use it for verification
                if (accountDto.AccountID > 0)
                {
                    var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountDto.AccountID);
                    if (account == null)
                        return false;

                    return account.Password == accountDto.Password;
                }
                // Otherwise use username and password
                else if (!string.IsNullOrWhiteSpace(accountDto.Username))
                {
                    return await _unitOfWork.AccountRepository.Login(_mapper.Map<Account>(accountDto));
                }
                else
                {
                    throw new ArgumentException("Either AccountID or Username must be provided");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify password: {ex.Message}", ex);
            }
        }
    }
}
