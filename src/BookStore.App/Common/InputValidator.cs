using System.Text.RegularExpressions;

namespace BookStore.App.Common
{
    public class InputValidator
    {
        private const int MaxRetries = 3;

        public static string GetValidString(string prompt, Func<string, bool> validationFunc, string errorMessage)
        {
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (validationFunc(input))
                {
                    return input;
                }

                attempts++;
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        public static int? GetValidInteger(string prompt, Func<int, bool> validationFunc, string errorMessage)
        {
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int result) && validationFunc(result))
                {
                    return result;
                }

                attempts++;
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        public static string GetNonEmptyString(string prompt)
        {
            return GetValidString(
                prompt,
                input => !string.IsNullOrWhiteSpace(input),
                "Input cannot be empty."
            );
        }

        public static string GetHiddenPassword(string prompt)
        {
            Console.Write(prompt);
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        public static string GetValidHiddenPassword(string prompt)
        {
            while (true)
            {
                string password = GetHiddenPassword(prompt);
                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Input cannot be empty.");
                    continue;
                }
                return password;
            }
        }

        public static string GetValidEmail(string prompt)
        {
            return GetValidString(
                prompt,
                input => string.IsNullOrWhiteSpace(input) || IsValidEmail(input),
                "Please enter a valid email address or leave it empty."
            );
        }

        public static string GetValidPhoneNumber(string prompt)
        {
            return GetValidString(
                prompt,
                input => string.IsNullOrWhiteSpace(input) || IsValidPhoneNumber(input),
                "Please enter a valid phone number or leave it empty."
            );
        }

        public static string GetValidMenuChoice(string prompt, string[] validOptions)
        {
            return GetValidString(
                prompt,
                input => validOptions.Contains(input),
                $"Please enter a valid option: {string.Join(", ", validOptions)}"
            );
        }

        public static string GetPasswordWithConfirmation(
        string prompt,
        string confirmPrompt,
        Func<string, bool> validationFunc,
        string errorMessage)
        {
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                string password = GetHiddenPassword(prompt);

                if (!validationFunc(password))
                {
                    attempts++;
                    Console.WriteLine(errorMessage);
                    Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
                    continue;
                }

                string confirmPassword = GetHiddenPassword(confirmPrompt);

                if (password == confirmPassword)
                {
                    return password;
                }

                attempts++;
                Console.WriteLine("Passwords do not match.");
                Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        public static async Task<string> GetUniqueUsername(string prompt, Func<string, Task<bool>> uniqueCheckFunc)
        {
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                Console.Write(prompt);
                string username = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                {
                    attempts++;
                    Console.WriteLine("Username cannot be empty.");
                    Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
                    continue;
                }

                bool isUnique = await uniqueCheckFunc(username);
                if (isUnique)
                {
                    return username;
                }

                attempts++;
                Console.WriteLine("Username already exists. Please choose a different username.");
                Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        public static string GetStrongPassword(string prompt)
        {
            return GetValidString(
                prompt,
                input => IsStrongPassword(input),
                "Password must be at least 6 characters long and contain at least one letter and one number."
            );
        }

        public static decimal? GetValidDecimal(string prompt, Func<decimal, bool> validationFunc, string errorMessage)
        {
            int attempts = 0;
            const int maxRetries = 3;

            while (attempts < maxRetries)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (decimal.TryParse(input, out decimal result) && validationFunc(result))
                {
                    return result;
                }

                attempts++;
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Attempts remaining: {maxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                return Regex.IsMatch(phoneNumber, @"^[\d\s\-$$$$]+$");
            }
            catch
            {
                return false;
            }
        }

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c))
                    hasLetter = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                if (hasLetter && hasDigit)
                    return true;
            }

            return false;
        }

        public static DateTime? GetValidDate(string prompt, string errorMessage)
        {
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }

                attempts++;
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Attempts remaining: {MaxRetries - attempts}");
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }
    }
}
