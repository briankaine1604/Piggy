using System;
using System.Collections.Generic;
using System.Text;

namespace PiggyBank;

public enum TransferResult
{
    Success,
    AmountInvalid,
    SameAccount,
    RecipientInvalid,
    InsufficientFunds
}
