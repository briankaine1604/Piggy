using System;
using System.Collections.Generic;
using System.Text;

namespace Piggy_Bank
{
    public class Customer
    {
        public string Name { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
    }
    public  class Account
    {
        public string AccountNumber { get; set; } = String.Empty;

        public decimal Balance { get; private set; } = 0.0m;

        public Customer AccountHolder { get; set; } = new Customer();
        public Account(string accountNumber, Customer holder, decimal openingBalance)
        {
            AccountNumber = accountNumber;
            AccountHolder = holder;
            Balance = openingBalance;
        }



        public bool TryDebit(decimal amount)
        {
            if (amount <= 0 || Balance < amount)
                return false;

            Balance -= amount;
            return true;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Balance += amount;
        }




    }
    public class Bank
    {

        public Bank()
        {
            var seeds = new[]
               {
                new Account("0000000998", new Customer { Name = "Ada Obi", City = "Lagos" }, 50000m),
                new Account("0000000999", new Customer { Name = "Chidi Okeke", City = "Abuja" }, 150000m),
                new Account("0000001000", new Customer { Name = "Tola Bello", City = "Ibadan" }, 500m),
               };

            foreach (var account in seeds)
                Accounts.Add(account.AccountNumber, account);
        }
        public Dictionary<string,Account> Accounts { get; set; } = new Dictionary<string, Account>();

        public int AccountNumberCounter = 1000;


        public void CheckBalance(Account account)
        {
            Console.WriteLine($"Your balance is: NGN {account.Balance:N2}");
        }


        public bool Transfer(Account sender, string recipientNumber, decimal amount)
        {
            if (!Accounts.TryGetValue(recipientNumber, out Account? recipient))
            {
                Console.WriteLine("Account not found. Please check the account number.");
                return false;
            }

            if (sender.AccountNumber == recipient.AccountNumber)
            {
                Console.WriteLine("You cannot transfer money to your own account.");
                return false;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Transfer amount must be greater than zero.");
                return false;
            }

            if (sender.Balance < amount)
            {
                Console.WriteLine(
                    $"Insufficient funds. Your balance is NGN {sender.Balance:N2}, but you attempted to transfer NGN {amount:N2}.");
                return false;
            }

            if (!sender.TryDebit(amount))
                return false;

            recipient.Credit(amount);

            return true;
        }


        public Account CreateAccount(string name, string city)
        {
            AccountNumberCounter++;
            string number = AccountNumberCounter.ToString("D10");
            var customer = new Customer { Name = name, City = city };
            var account = new Account(number, customer, 10000m);
            Accounts.Add(number, account);
            return account;
        }


    }
}
