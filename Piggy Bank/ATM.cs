using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PiggyBank
{
    public class ATM
    {
        private readonly Bank _bank;

        private static readonly List<DataPlan> _dataPlans = new()
        {
            new("500MB - 7 days", 500m),
            new("1GB - 30 days", 1000m),
            new("2GB - 30 days", 1500m),
            new("5GB - 30 days", 3000m),
            new("10GB - 30 days", 5000m)
        };

        public ATM(Bank bank)
        {
            _bank = bank;
        }

        private void BuyAirtime(Account account)
        {
            string? phone = ReadPhoneNumber();
            if (phone == null) return;

            string? network = ChooseNetwork();

            if (network == null)
            {
                Console.WriteLine("Invalid network.");
                return;
            }

            Console.Write("How much do you want to recharge? ");
            if (!TryReadAmount(Console.ReadLine(), out decimal amount))
                return;

            if (amount != Math.Round(amount, 2))
            {
                Console.WriteLine("Amount can have at most 2 decimal places.");
                return;
            }

            bool success = account.TryDebit(amount, $"Airtime for {phone} ({network})", "Airtime");

            if (success)
            {
                Console.WriteLine($"Airtime purchase successful. Amount: {amount:N2} for {phone} ({network})");
                Console.WriteLine($"Remaining balance: {account.Balance:N2}");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }

        private void BuyData(Account account)
        {
            string? phone = ReadPhoneNumber();
            if (phone == null) return;

            string? network = ChooseNetwork();
            if (network == null)
            {
                Console.WriteLine("Invalid network.");
                return;
            }

            Console.WriteLine("\nSelect data plan:");
            for (int i = 0; i < _dataPlans.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_dataPlans[i].Name} - {_dataPlans[i].Price:N2}");
            }
            Console.Write("Choose plan: ");

            if (!int.TryParse(Console.ReadLine(), out int planChoice)
                || planChoice < 1 || planChoice > _dataPlans.Count)
            {
                Console.WriteLine("Invalid plan.");
                return;
            }

            DataPlan plan = _dataPlans[planChoice - 1];

            bool success = account.TryDebit(plan.Price, $"{plan.Name} data for {phone} ({network})", "Data");

            if (success)
            {
                Console.WriteLine($"Data purchase successful: {plan.Name} for {phone} ({network})");
                Console.WriteLine($"Remaining balance: {account.Balance:N2}");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }

        private string? ChooseNetwork()
        {
            Console.WriteLine("Select network:");
            Console.WriteLine("1. MTN");
            Console.WriteLine("2. Airtel");
            Console.WriteLine("3. Glo");
            Console.WriteLine("4. 9mobile");
            Console.Write("Choose network: ");

            return Console.ReadLine()?.Trim() switch
            {
                "1" => "MTN",
                "2" => "Airtel",
                "3" => "Glo",
                "4" => "9mobile",
                _ => null
            };
        }

        private string ReadName(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? "";

                if (input.Length >= 2 && input.All(c => char.IsLetter(c) || c == ' ' || c == '-' || c == '\''))
                    return input;

                Console.WriteLine("Use letters only (spaces, hyphens, apostrophes allowed), at least 2 characters.");
            }
        }

        private string? ReadPhoneNumber()
        {
            Console.Write("Enter phone number: ");
            string phone = Console.ReadLine()?.Trim().Replace(" ", "").Replace("-", "") ?? "";

            if (phone.StartsWith("+234"))
                phone = "0" + phone.Substring(4);
            else if (phone.StartsWith("234"))
                phone = "0" + phone.Substring(3);
            else if (phone.Length == 10 && !phone.StartsWith("0"))
                phone = "0" + phone;

            if (phone.Length != 11 || !phone.All(char.IsDigit) || !phone.StartsWith("0"))
            {
                Console.WriteLine("Enter a valid Nigerian number, e.g. 08012345678, 8012345678 or +2348012345678.");
                return null;
            }

            return phone;
        }

        private const decimal MaxAmount = 10_000_000m;

        private bool TryReadAmount(string? input, out decimal amount)
        {
            if (!decimal.TryParse(input, out amount))
            {
                Console.WriteLine("Invalid amount. Please enter a number.");
                return false;
            }
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
                return false;
            }
            if (amount != Math.Round(amount, 2))
            {
                Console.WriteLine("Amount can have at most 2 decimal places.");
                return false;
            }
            if (amount > MaxAmount)
            {
                Console.WriteLine($"Amount cannot exceed {MaxAmount:N2}.");
                return false;
            }
            return true;
        }
        private void Withdraw(Account account)
        {
            Console.Write("How much do you want to withdraw? ");
            if (!TryReadAmount(Console.ReadLine(), out decimal amount))
                return;

            bool success = account.TryDebit(amount, "Cash withdrawal", "Withdrawal");

            if (success)
            {
                Console.WriteLine($"Withdrawal successful. Amount: {amount:N2}");
                Console.WriteLine($"Remaining balance: {account.Balance:N2}");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }

        private void Deposit(Account account)
        {
            Console.Write("How much do you want to deposit? ");
            if (!TryReadAmount(Console.ReadLine(), out decimal amount))
                return;

            account.Credit(amount, "Cash deposit", "Deposit");

            Console.WriteLine($"Deposit successful. Amount: {amount:N2}");
            Console.WriteLine($"New balance: {account.Balance:N2}");
        }

        private void ShowTransactionHistory(Account account)
        {
            if (account.Transactions.Count == 0)
            {
                Console.WriteLine("You don't have any transactions yet.");
                return;
            }

            Console.WriteLine($"\n{"Date",-18}{"Type",-12}{"Amount",14}   Description");
            Console.WriteLine(new string('-', 95));

            foreach (Transaction t in account.Transactions)
            {
                Console.WriteLine($"{t.Date,-18:dd MMM yyyy HH:mm}{t.Type,-12}{t.Amount,14:N2}   {t.Description}");
            }
        }
        private void Transfer(Account sender)
        {
            Console.Write("Enter recipient account number: ");
            string recipientAccountNumber = Console.ReadLine() ?? "";

            Console.Write("How much do you want to transfer? ");
            if (!TryReadAmount(Console.ReadLine(), out decimal amount))
                return;

            TransferResult result = _bank.Transfer(sender, recipientAccountNumber, amount);

            switch (result)
            {
                case TransferResult.Success:
                    Console.WriteLine("Transfer successful.");
                    Console.WriteLine($"Remaining balance: {sender.Balance:N2}");
                    break;

                case TransferResult.AmountInvalid:
                    Console.WriteLine("Amount must be greater than zero.");
                    break;

                case TransferResult.SameAccount:
                    Console.WriteLine("You can't transfer money to your own account.");
                    break;

                case TransferResult.RecipientInvalid:
                    Console.WriteLine("Recipient account not found. Check the account number and try again.");
                    break;

                case TransferResult.InsufficientFunds:
                    Console.WriteLine("Insufficient funds for this transfer.");
                    break;
            }
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n=== ATM ===");
                Console.WriteLine("1. Create account");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateAccount();
                        break;

                    case "2":
                        Login();
                        break;

                    case "3":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void AccountMenu(Account account)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.WriteLine($"\nWelcome, {account.AccountHolder.Name}");
                Console.WriteLine("1. Check balance");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Transfer");
                Console.WriteLine("5. Transaction history");
                Console.WriteLine("6. Buy Airtime");
                Console.WriteLine("7. Buy Data");
                Console.WriteLine("8. Logout");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine($"Balance: {account.Balance:N2}");
                        break;

                    case "2":
                        Deposit(account);
                        break;

                    case "3":
                        Withdraw(account);
                        break;

                    case "4":
                        Transfer(account);
                        break;

                    case "5":
                        ShowTransactionHistory(account);
                        break;

                    case "6":
                        BuyAirtime(account);
                        break;

                    case "7":
                        BuyData(account);
                        break;

                    case "8":
                        loggedIn = false;
                        Console.WriteLine("Logged out successfully.");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void CreateAccount()
        {
            string name = ReadName("Enter your name: ");
            string city = ReadName("Enter your city: ");

            string pin;
            while (true)
            {
                Console.Write("Choose a PIN (4 digits): ");
                pin = Console.ReadLine()?.Trim() ?? "";

                if (pin.Length == 4 && pin.All(char.IsDigit))
                    break;

                Console.WriteLine("PIN must contain exactly four digits.");
            }

            decimal deposit;
            while (true)
            {
                Console.Write("How much do you want to deposit?: ");
                if (TryReadAmount(Console.ReadLine(), out deposit))
                    break;
            }

            Account account = _bank.CreateAccount(name, city, pin, deposit);

            Console.WriteLine("\nAccount created successfully!");
            Console.WriteLine($"Account number: {account.AccountNumber}");
            Console.WriteLine($"Account holder: {account.AccountHolder.Name}");
            Console.WriteLine($"Opening balance: {account.Balance:N2}");
        }


        private void Login()
        {
            Console.Write("Enter your account number: ");
            string accountNumber = Console.ReadLine() ?? "";

            Console.Write("Enter your PIN: ");
            string pin = Console.ReadLine() ?? "";

            Account? account = _bank.Authenticate(accountNumber, pin);

            if (account == null)
            {
                Console.WriteLine("Invalid account number or PIN.");
                return;
            }

            AccountMenu(account);
        }
    }
    public record DataPlan(string Name, decimal Price);

}
