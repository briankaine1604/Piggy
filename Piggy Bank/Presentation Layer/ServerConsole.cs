namespace PiggyBank;

public class ServerConsole
{
    public void DisplayServerStarted(string address)
    {
        Console.WriteLine($"[PiggyBank] HTTP server listening on {address}");
    }

    public void DisplayServerStartFailed(string message, int errorCode)
    {
        Console.WriteLine($"[PiggyBank] could not start the HTTP server: {message} (error {errorCode})");
    }

    public void DisplayIncomingTransfer(string accountNumber, decimal amount, decimal newBalance)
    {
        Console.WriteLine($"[PiggyBank] credited {amount:N2} to {accountNumber} (balance now {newBalance:N2})");
    }

    public void DisplayTransferRejected(string reason)
    {
        Console.WriteLine($"[PiggyBank] transfer rejected: {reason}");
    }
}
