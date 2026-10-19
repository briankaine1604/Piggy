using PiggyBank;
using System.Globalization;
using System.Linq;


var bank = new Bank();
BankSeeder.Seed(bank);

ATM atm = new(bank);
atm.Run();



//Account newAccount = bank.CreateAccount("Brian", "Lagos", "5678");

//Console.WriteLine($"Account created: {newAccount.AccountNumber}");
//Console.WriteLine($"Account holder: {newAccount.AccountHolder.Name}");
//Console.WriteLine($"Balance: {newAccount.Balance:N2}");