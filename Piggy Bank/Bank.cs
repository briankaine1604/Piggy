using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyBank
{
    public class Bank
    {
        private readonly Dictionary<string, Account> _accounts = new();

        private int _nextAccountNumber = 1001;

        public void AddAccount(Account account)
        {
           //if (_accounts.ContainsKey(account.AccountNumber)) {
           //     throw new InvalidOperationException("Account Number already exists");
           // }

           // _accounts.Add(account.AccountNumber, account);

        //My method above changed it cause its 2 diff operations which is more time and more work


            if (!_accounts.TryAdd(account.AccountNumber, account))
            {
                throw new InvalidOperationException("Account number already exists.");
            }
        }

        public Account? GetAccount(string accountNumber)
        {
            _accounts.TryGetValue(accountNumber, out Account? account);
            return account;
        }

        public Account? Authenticate(string accountNumber, string pin)
        {
            var account = GetAccount(accountNumber);

            if (account == null) return null;
            if (account.AccountPin != pin) return null;

            return account;
        }

        public string GenerateAccountNumber()
        {
            string accountNumber = _nextAccountNumber.ToString("D10");

            _nextAccountNumber++;

            return accountNumber;
        }

        public Account CreateAccount(string name, string city, string pin)
        {
            Customer customer = new Customer(name, city);

            string accountNumber = GenerateAccountNumber();

            Account account = new(accountNumber, customer, 5000m, pin);

            AddAccount(account);

            return account;

        }

        public bool Transfer(Account sender, string recipientAccountNumber, decimal amount)
        {
            if (amount <= 0)
            {
                //Console.WriteLine("Amount must be greater than zero");
                return false;
            }
            var recipient = GetAccount(recipientAccountNumber);
            if (recipient == null)
            {
                //Console.WriteLine("Account does not exist");
                return false;
            }
           
            bool success = sender.TryDebit(amount);

            if (success == false)
            {
                //Console.WriteLine("Insufficient Funds");
                return false;
            }
            recipient.Credit(amount);
            return true;

        }
    }


}