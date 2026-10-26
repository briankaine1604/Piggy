using System;
using System.Collections.Generic;
using System.Text;

namespace UnityBank;

public enum TransferResult
{
    Success,
    AmountInvalid,
    SameAccount,
    RecipientInvalid,
    InsufficientFunds
}
