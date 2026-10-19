using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyBank
{
    public class ATM
    {
        private readonly Bank _bank;

        public ATM(Bank bank)
        {
            _bank = bank;
        }

        private void Withdraw(Account account)
        {
            Console.Write("How much do you want to withdraw? ");
            string? amountInString = Console.ReadLine();

            bool isValidAmount = decimal.TryParse(amountInString, out decimal amount);

            if (!isValidAmount)
            {
                Console.WriteLine("Invalid amount. Please enter a number.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
                return;
            }

            bool success = account.TryDebit(amount);

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


        private void Transfer(Account sender)
        {
            Console.Write("Enter recipient account number: ");
            string recipientAccountNumber = Console.ReadLine() ?? "";

            Console.Write("How much do you want to transfer? ");
            string? amountInput = Console.ReadLine();

            if (!decimal.TryParse(amountInput, out decimal amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            bool success = _bank.Transfer(sender, recipientAccountNumber, amount);

            if (success)
            {
                Console.WriteLine("Transfer successful.");
                Console.WriteLine($"Remaining balance: {sender.Balance:N2}");
            }
            else
            {
                Console.WriteLine("Transfer failed.");
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
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Transfer");
                Console.WriteLine("4. Transaction history");
                Console.WriteLine("5. Logout");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine($"Balance: {account.Balance:N2}");
                        break;

                    case "2":
                        Withdraw(account);
                        break;

                    case "3":
                        Transfer(account);
                        break;

                    case "4":
                        // Transaction history will go here.
                        break;

                    case "5":
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
            Console.Write("Enter your name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter your city: ");
            string city = Console.ReadLine() ?? "";

            Console.Write("Choose a PIN: ");
            string pin = Console.ReadLine() ?? "";

            Account account = _bank.CreateAccount(name, city, pin);

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
}
