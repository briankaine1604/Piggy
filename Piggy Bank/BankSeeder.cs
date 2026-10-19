using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyBank;
public static class BankSeeder
{
    public static void Seed(Bank bank)
    {
        // Register sample accounts here.
        Customer ada = new("Ada","Benin");
        Account account = new("0000000998", ada, 50000m, "1234");
        bank.AddAccount(account);

    }
}
