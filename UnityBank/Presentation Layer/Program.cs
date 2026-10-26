using System.Globalization;
using System.Linq;
using UnityBank;
using UnityBank.Http;


Console.Title = "UnityBank ATM  (Bank B)";

var bank = new Bank();
ATMConsole aTMConsole = new();
AdminConsole adminConsole = new();
ServerConsole serverConsole = new();

Admin admin = new("admin", "password");
BankSeeder.Seed(bank);

// Talks to PiggyBank, and listens for PiggyBank talking to us.
var httpClient = new BankHttpClient("http://localhost:5001/");
var httpServer = new BankHttpServer(bank, serverConsole, "http://localhost:5002/");
_ = httpServer.Start();

ATM atm = new(bank, aTMConsole, adminConsole, admin, httpClient);
atm.Run();
