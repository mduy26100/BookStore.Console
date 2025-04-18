using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task ChangePassword(int id, string newPassword)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid account ID");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty");

            try
            {
                var accountUser = await _context.Accounts.FindAsync(id);
                if (accountUser == null)
                {
                    throw new Exception($"Account with ID {id} not found");
                }

                accountUser.Password = newPassword;
                _context.Accounts.Update(accountUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to change password: {ex.Message}", ex);
            }
        }

        public async Task<Account> GetAccountDetails(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            try
            {
                Account accountUser;

                if (account.AccountID > 0)
                {
                    accountUser = await _context.Accounts.FindAsync(account.AccountID);
                }
                else if (!string.IsNullOrWhiteSpace(account.Username))
                {
                    accountUser = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.Username == account.Username);
                }
                else
                {
                    throw new Exception("Insufficient information to find account");
                }

                if (accountUser == null)
                {
                    throw new Exception("Account not found");
                }

                return accountUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get account details: {ex.Message}", ex);
            }
        }

        public async Task<bool> Login(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (string.IsNullOrWhiteSpace(account.Username) || string.IsNullOrWhiteSpace(account.Password))
                throw new ArgumentException("Username and password are required");

            try
            {
                return await _context.Accounts.AnyAsync(x =>
                    x.Username == account.Username &&
                    x.Password == account.Password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}", ex);
            }
        }

        public async Task UpdateAddress(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            try
            {
                var accountUser = await _context.Accounts.FindAsync(account.AccountID);
                if (accountUser == null)
                {
                    throw new Exception($"Account with ID {account.AccountID} not found");
                }

                accountUser.Address = account.Address;
                _context.Accounts.Update(accountUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update address: {ex.Message}", ex);
            }
        }

        public async Task UpdateName(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            if (string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentException("Name cannot be empty");

            try
            {
                var accountUser = await _context.Accounts.FindAsync(account.AccountID);
                if (accountUser == null)
                {
                    throw new Exception($"Account with ID {account.AccountID} not found");
                }

                accountUser.Name = account.Name;
                _context.Accounts.Update(accountUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update name: {ex.Message}", ex);
            }
        }

        public async Task UpdatePhoneNumber(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.AccountID <= 0)
                throw new ArgumentException("Invalid account ID");

            try
            {
                var accountUser = await _context.Accounts.FindAsync(account.AccountID);
                if (accountUser == null)
                {
                    throw new Exception($"Account with ID {account.AccountID} not found");
                }

                accountUser.PhoneNumber = account.PhoneNumber;
                _context.Accounts.Update(accountUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update phone number: {ex.Message}", ex);
            }
        }
    }
}
