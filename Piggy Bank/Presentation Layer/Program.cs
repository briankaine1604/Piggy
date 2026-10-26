using PiggyBank;
using PiggyBank.Http;
using System.Globalization;
using System.Linq;


Console.Title = "Piggy Bank ATM  (Bank A)";

var bank = new Bank();
ATMConsole aTMConsole = new();
AdminConsole adminConsole = new();
ServerConsole serverConsole = new();


Admin admin = new("admin", "password");
BankSeeder.Seed(bank);

// Talks to UnityBank, and listens for UnityBank talking to us.
var httpClient = new BankHttpClient("http://localhost:5002/");
var httpServer = new BankHttpServer(bank, serverConsole, "http://localhost:5001/");
_ = httpServer.Start();

ATM atm = new(
    bank,
    aTMConsole,
    adminConsole,
    admin,
    httpClient);

atm.Run();
