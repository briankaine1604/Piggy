using PiggyBank;
using System.Globalization;
using System.Linq;


var bank = new Bank();
ATMConsole aTMConsole = new();
AdminConsole adminConsole = new();
Admin admin = new("admin", "password");
BankSeeder.Seed(bank);

ATM atm = new(bank, aTMConsole, adminConsole, admin);
atm.Run();