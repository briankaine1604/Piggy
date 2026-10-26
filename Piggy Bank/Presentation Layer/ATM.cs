using PiggyBank;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PiggyBank
{
    public class ATM
    {
        private readonly Bank _bank;

        private readonly ATMConsole _atmConsole;

        private readonly AdminConsole _adminConsole;

        private readonly Admin _admin;


        public ATM(Bank bank, ATMConsole atmConsole, AdminConsole adminConsole, Admin admin)
        {
            _bank = bank;
            _atmConsole = atmConsole;
            _adminConsole = adminConsole;
            _admin = admin;
        }

        private void AdminLogin()
        {
            string username = _adminConsole.ReadUsername();
            string password = _adminConsole.ReadPassword();

            bool isAuthenticated = _admin.Authenticate(username, password);

            if (!isAuthenticated)
            {
                _adminConsole.DisplayInvalidAdminLogin();
                return;
            }

            _adminConsole.DisplayAdminLoginSuccess();
            AdminMenu();
        }


        private void AdminMenu()
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                _adminConsole.DisplayAdminMenu();

                string? choice = _atmConsole.ReadMenuChoice();

                switch (choice)
                {
                    case "1":
                        ViewAllAccounts();
                        break;

                    case "2":
                        FindAccount();
                        break;

                    case "3":
                        ManageAccount();
                        break;

                    case "4":
                        loggedIn = false;
                        _atmConsole.DisplayLogout();
                        break;

                    default:
                        _atmConsole.DisplayInvalidOption();
                        break;
                }
            }
        }

        private void BuyAirtime(Account account)
        {
            string? phone = _atmConsole.ReadPhoneNumber();
            if (phone == null) return;

            string? network = _atmConsole.ChooseNetwork();
            if (network == null)
            {
                _atmConsole.DisplayInvalidNetwork();
                return;
            }

            if (!_atmConsole.TryReadAmount(
                "How much do you want to recharge? ",
                out decimal amount))
                return;

            bool success = account.TryDebit(
                amount,
                $"Airtime for {phone} ({network})",
                "Airtime");

            if (success)
                _atmConsole.DisplayAirtimePurchaseSuccess(amount, phone, network, account.Balance);
            else
                _atmConsole.DisplayInsufficientFunds();
        }

        private void BuyData(Account account)
        {
            string? phone = _atmConsole.ReadPhoneNumber();
            if (phone == null) return;

            string? network = _atmConsole.ChooseNetwork();
            if (network == null)
            {
                _atmConsole.DisplayInvalidNetwork();
                return;
            }

            DataPlan? plan = _atmConsole.ChooseDataPlan();
            if (plan == null) return;

            bool success = account.TryDebit(
                plan.Price,
                $"{plan.Name} data for {phone} ({network})",
                "Data");

            if (success)
            {
                _atmConsole.DisplayDataPurchaseSuccess(
                    plan, phone, network, account.Balance);
            }
            else
            {
                _atmConsole.DisplayInsufficientFunds();
            }
        }      

        private void Withdraw(Account account)
        {
            if (!_atmConsole.TryReadAmount("How much do you want to withdraw? ", out decimal amount))
                return;

            bool success = account.TryDebit(amount, "Cash withdrawal", "Withdrawal");

            if (success)
                _atmConsole.DisplayWithdrawalSuccess(amount, account.Balance);
            else
                _atmConsole.DisplayInsufficientFunds();
        }

        private void Deposit(Account account)
        {
            if (!_atmConsole.TryReadAmount("How much do you want to deposit? ", out decimal amount))
                return;

            account.Credit(amount, "Cash deposit", "Deposit");

            _atmConsole.DisplayDepositSuccess(amount, account.Balance);
        }

        private void ShowTransactionHistory(Account account)
        {
            if (account.Transactions.Count == 0)
            {
                _atmConsole.DisplayNoTransactions();
                return;
            }

            _atmConsole.DisplayTransactionHistory(account.Transactions);
        }
        private void Transfer(Account sender)
        {
            string recipientAccountNumber = _atmConsole.ReadRecipientAccountNumber();

            if (!_atmConsole.TryReadAmount(
                "How much do you want to transfer? ",
                out decimal amount))
            {
                return;
            }

            TransferResult result = _bank.Transfer(
                sender, recipientAccountNumber, amount);

            switch (result)
            {
                case TransferResult.Success:
                    _atmConsole.DisplayTransferSuccess(sender.Balance);
                    break;

                case TransferResult.AmountInvalid:
                    _atmConsole.DisplayInvalidTransferAmount();
                    break;

                case TransferResult.SameAccount:
                    _atmConsole.DisplaySameAccountError();
                    break;

                case TransferResult.RecipientInvalid:
                    _atmConsole.DisplayRecipientNotFound();
                    break;

                case TransferResult.InsufficientFunds:
                    _atmConsole.DisplayInsufficientFunds();
                    break;
            }
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                _atmConsole.DisplayMainMenu();

                string? choice = _atmConsole.ReadMenuChoice();

                switch (choice)
                {
                    case "1":
                        CreateAccount();
                        break;

                    case "2":
                        Login();
                        break;

                    case "3":
                        AdminLogin();
                        break;

                    case "4":
                        isRunning = false;
                        break;

                    default:
                        _atmConsole.DisplayInvalidOption();
                        break;
                }
            }
        }

        private void AccountMenu(Account account)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                _atmConsole.DisplayAccountMenu(account);

                string? choice = _atmConsole.ReadMenuChoice();

                switch (choice)
                {
                    case "1":
                        _atmConsole.DisplayBalance(account.Balance);
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
                        _atmConsole.DisplayLogout();
                        break;

                    default:
                        _atmConsole.DisplayInvalidOption();
                        break;
                }
            }
        }

        private void CreateAccount()
        {
            string name = _atmConsole.ReadName("Enter your name: ");
            string city = _atmConsole.ReadName("Enter your city: ");

            string pin = _atmConsole.ReadNewPin();

            decimal deposit;
            while (!_atmConsole.TryReadAmount("How much do you want to deposit?: ", out deposit))
            {
                // keep asking until the amount passes validation
            }

            Account account = _bank.CreateAccount(name, city, pin, deposit);

            _atmConsole.OnAccountCreatedSuccess(account);
        }


        private void Login()
        {
            string accountNumber = _atmConsole.ReadAccountNumber();
            string pin = _atmConsole.ReadPin();

            Account? account = _bank.Authenticate(accountNumber, pin);

            if (account == null)
            {
                _atmConsole.DisplayInvalidLogin();
                return;
            }

            if (account.IsFrozen)
            {
                _atmConsole.DisplayAccountFrozen();
                return;
            }

            AccountMenu(account);
        }

        private void ViewAllAccounts()
        {
            var accounts = _bank.GetAccounts();

            _adminConsole.DisplayAccounts(accounts);
        }

        private void FindAccount()
        {
            string accountNumber = _atmConsole.ReadAccountNumber();

            Account? account = _bank.GetAccount(accountNumber);

            if (account == null)
            {
                _adminConsole.DisplayAccountNotFound();
                return;
            }

            _adminConsole.DisplayAccount(account);
        }

        private void ManageAccount()
        {
            var accounts = _bank.GetAccounts().ToList();

            if (accounts.Count == 0)
            {
                _adminConsole.DisplayNoAccounts();
                return;
            }

            _adminConsole.DisplayAccountSelection(accounts);

            int? selection = _adminConsole.ChooseAccountSelection(accounts.Count);

            if (selection == null)
            {
                _atmConsole.DisplayInvalidOption();
                return;
            }

            if (selection == 0)
            {
                return;
            }

            ManageAccountMenu(accounts[selection.Value - 1]);
        }

        private void ManageAccountMenu(Account account)
        {
            bool isManaging = true;

            while (isManaging)
            {
                _adminConsole.DisplayManageAccountMenu(account);

                string? choice = _atmConsole.ReadMenuChoice();

                switch (choice)
                {
                    case "1":
                        if (account.IsFrozen)
                        {
                            account.Unfreeze();
                            _adminConsole.DisplayUnfreezeSuccess();
                        }
                        else
                        {
                            account.Freeze();
                            _adminConsole.DisplayFreezeSuccess();
                        }

                        break;

                    case "2":
                        isManaging = false;
                        break;

                    default:
                        _atmConsole.DisplayInvalidOption();
                        break;
                }
            }
        }

        
    }
 

}
