namespace PassGen.GlobalTool;

public sealed class LoggedException : Exception
{
    public LoggedException(string message) : base(message) {}
}
