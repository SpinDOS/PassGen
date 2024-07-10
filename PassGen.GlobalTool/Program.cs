#nullable enable
using PassGen.Lib;

namespace PassGen.GlobalTool;

internal static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var passGenArgsParser = new PassGenArgsParser();
            var passGenArgs = passGenArgsParser.ParseArgs(args);
            if (passGenArgs == null)
                return;

            var target = passGenArgs.Value.Target;
            var salt = passGenArgs.Value.Salt ?? new SaltResolver().ResolveSalt(passGenArgs.Value.Salt);
            if (salt == null)
            {
                passGenArgsParser.PrintSaltHelp();
                return;
            }

            ValidateTargetSalt(target, "target");
            ValidateTargetSalt(salt, "salt");

            var password = new PasswordGenerator().GeneratePassword(target, salt);
            Console.WriteLine(password);
        }
        catch (LoggedException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
    }

    private static void ValidateTargetSalt(string? str, string name)
    {
        if (string.IsNullOrEmpty(str))
            throw new LoggedException($"{name} is empty");
        if (str.Contains(Environment.NewLine))
            throw new LoggedException($"{name} contains multiple lines");
    }
}
