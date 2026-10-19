using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyBank
{
    public class Account
    {
        public string AccountNumber { get; private set; } = String.Empty;

        public decimal Balance { get; private set; } = 0.0m;

        public string AccountPin { get; private set; } = String.Empty;

        public Customer AccountHolder { get; private set; }
        public Account(string accountNumber, Customer holder, decimal openingBalance, string accountPin)
        {
            AccountNumber = accountNumber;
            AccountHolder = holder;
            Balance = openingBalance;
            AccountPin = accountPin;
        }



        public bool TryDebit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount), "Withdrawal amount must be greater than zero.");

            if (amount > Balance)
                return false;

            Balance -= amount;
            return true;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

            Balance += amount;
        }




    }
}
