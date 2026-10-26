namespace UnityBank;

public class ServerConsole
{
    public void DisplayServerStarted(string address)
    {
        Console.WriteLine($"[UnityBank] HTTP server listening on {address}");
    }

    public void DisplayServerStartFailed(string message, int errorCode)
    {
        Console.WriteLine($"[UnityBank] could not start the HTTP server: {message} (error {errorCode})");
    }

    public void DisplayIncomingTransfer(string accountNumber, decimal amount, decimal newBalance)
    {
        Console.WriteLine($"[UnityBank] credited {amount:N2} to {accountNumber} (balance now {newBalance:N2})");
    }

    public void DisplayTransferRejected(string reason)
    {
        Console.WriteLine($"[UnityBank] transfer rejected: {reason}");
    }
}
