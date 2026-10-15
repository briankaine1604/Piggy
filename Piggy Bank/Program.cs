using Piggy_Bank;
using System.Globalization;
using System.Linq;


var bank = new Bank();
string name = ReadRequired("Please input your name: ");
string city = ReadRequired("Please input your city: ");
Account currentUser = bank.CreateAccount(name, city);

Console.WriteLine();
Console.WriteLine("================================");
Console.WriteLine("       WELCOME TO PIGGY BANK");
Console.WriteLine("================================");
Console.WriteLine($"Customer: {currentUser.AccountHolder.Name}");
Console.WriteLine($"Branch:   {currentUser.AccountHolder.City}");
Console.WriteLine($"Date:     {DateTime.Today:dd MMMM yyyy}");
Console.WriteLine("================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Check balance");
    Console.WriteLine("2. Transfer");
    Console.WriteLine("3. Exit");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine()?.Trim() ?? "";

    switch (choice)
    {
        case "1":
            Console.WriteLine($"Your balance is: NGN {currentUser.Balance:N2}");
            break;


        case "2":
            Console.Write("Enter recipient account number: ");
            string recipientNumber = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter transfer amount: NGN ");
            string amountInput = Console.ReadLine()?.Trim() ?? "";

            if (!decimal.TryParse(amountInput, out decimal amount))
            {
                Console.WriteLine("Invalid amount.");
                break;
            }

            Console.Write($"Confirm transfer of NGN {amount:N2}? (yes/no): ");
            string confirmation = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";

            if (confirmation != "yes")
            {
                Console.WriteLine("Transfer cancelled.");
                break;
            }

            decimal previousBalance = currentUser.Balance;

            if (bank.Transfer(currentUser, recipientNumber, amount))
            {
                Console.WriteLine("Transfer successful!");
                bank.CheckBalance(currentUser);
            }
            else
            {
                Console.WriteLine("Transfer was not completed.");
            }

            break;


        case "3":
            Console.WriteLine("Thank you for banking with NovaBank.");
            return;

        default:
            Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
            break;
    }
}
static string ReadRequired(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.Trim() ?? "";
        if (input != "") return input;
        Console.WriteLine("Input cannot be empty. Try again.");
    }
}