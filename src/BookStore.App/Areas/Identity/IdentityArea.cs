using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;

namespace BookStore.App.Areas.Identity
{
    public class IdentityArea
    {
        private readonly IAccountService _accountService;

        public IdentityArea(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async void ViewProfile(AccountDto user)
        {
            Console.Clear();
            Console.WriteLine("+" + new string('=', 30) + "+");
            Console.WriteLine("|          Your Profile          |");
            Console.WriteLine("+" + new string('=', 30) + "+");
            Console.WriteLine($"| Name:     {user.Name.PadRight(20)}|");
            Console.WriteLine($"| Username: {user.Username.PadRight(20)}|");
            Console.WriteLine($"| Email:    {user.Email.PadRight(20)}|");
            Console.WriteLine($"| Phone:    {user.PhoneNumber.PadRight(20)}|");
            Console.WriteLine($"| Address:  {user.Address.PadRight(20)}|");
            Console.WriteLine("+" + new string('=', 30) + "+");
            await UpdateProfile(user);
        }

        public async Task UpdateProfile(AccountDto user)
        {
            Console.WriteLine("=== Update Profile ===");
            Console.WriteLine("1. Update Name");
            Console.WriteLine("2. Update Phone");
            Console.WriteLine("3. Update Address");
            Console.WriteLine("4. Change Password");
            Console.WriteLine("0. Back");

            string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4" });

            if (choice == null)
                return;

            switch (choice)
            {
                case "1":
                    await UpdateName(user);
                    break;
                case "2":
                    await UpdatePhone(user);
                    break;
                case "3":
                    await UpdateAddress(user);
                    break;
                case "4":
                    await ChangePassword(user);
                    break;
                case "0":
                    break;
            }
        }

        public async Task ViewAllAccounts()
        {
            Console.Clear();
            Console.WriteLine("+" + new string('=', 50) + "+");
            Console.WriteLine("|               All Accounts               |");
            Console.WriteLine("+" + new string('=', 50) + "+");

            var accounts = await _accountService.GetAllAccount();

            if (accounts == null || !accounts.Any())
            {
                Console.WriteLine("| No accounts found.                      |");
                Console.WriteLine("+" + new string('=', 50) + "+");
                return;
            }

            Console.WriteLine("| ID   | Username       | Name            | Role       |");
            Console.WriteLine("+" + new string('-', 50) + "+");

            foreach (var account in accounts)
            {
                Console.WriteLine($"| {account.AccountID,-4} | {account.Username,-14} | {account.Name,-14} | {account.Role,-10} |");
            }

            Console.WriteLine("+" + new string('=', 50) + "+");
        }

        public async Task UpdateName(AccountDto user)
        {
            string newName = InputValidator.GetNonEmptyString("Enter new name: ");

            if (newName == null)
                return;

            user.Name = newName;

            try
            {
                await _accountService.UpdateNameAccount(user);
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine("| Name updated successfully!             |");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
            catch (Exception ex)
            {
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine($"| Error updating name: {ex.Message.PadRight(20)}|");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
        }

        public async Task UpdatePhone(AccountDto user)
        {
            string newPhone = InputValidator.GetValidPhoneNumber("Enter new phone number: ");

            if (newPhone == null)
                return;

            user.PhoneNumber = newPhone;

            try
            {
                await _accountService.UpdatePhoneAccount(user);
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine("| Phone number updated successfully!           |");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
            catch (Exception ex)
            {
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine($"| Error updating phone number: {ex.Message.PadRight(20)}|");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
        }

        public async Task UpdateAddress(AccountDto user)
        {
            string newAddress = InputValidator.GetValidString("Enter new address: ",
                input => true,
                "Invalid address format.");

            if (newAddress == null)
                return;

            user.Address = newAddress;

            try
            {
                await _accountService.UpdateAddressAccount(user);
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine("| Address updated successfully!           |");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
            catch (Exception ex)
            {
                Console.WriteLine("+" + new string('=', 30) + "+");
                Console.WriteLine($"| Error updating address: {ex.Message.PadRight(20)}|");
                Console.WriteLine("+" + new string('=', 30) + "+");
            }
        }

        public async Task ChangePassword(AccountDto user)
        {
            string currentPassword = InputValidator.GetNonEmptyString("Enter current password: ");
            if (currentPassword == null) return;

            var verifyDto = new AccountDto
            {
                AccountID = user.AccountID,
                Username = user.Username,
                Password = currentPassword
            };

            try
            {
                bool isValidPassword = await _accountService.VerifyPassword(verifyDto);
                if (!isValidPassword)
                {
                    Console.WriteLine("Current password is incorrect.");
                    return;
                }

                string newPassword = InputValidator.GetPasswordWithConfirmation(
                    "Enter new password: ",
                    "Confirm new password: ",
                    pwd => !string.IsNullOrWhiteSpace(pwd) && pwd.Length >= 6,
                    "Password must be at least 6 characters long."
                );

                if (newPassword == null) return;

                await _accountService.ChangePasswordAccount(user.AccountID, newPassword);
                Console.WriteLine("Password changed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
            }
        }
    }
}
