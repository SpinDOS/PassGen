#nullable enable
namespace PassGen.GlobalTool;

public sealed class PassGenArgsParser
{
    public PassGenArgs? ParseArgs(string[] args)
    {
        args = args ?? Array.Empty<string>();
        if (IsHelp(args))
        {
            PrintUsageHelp();
            Console.WriteLine();
            PrintSaltHelp();
            return null;
        }
        else if (!AreValidArgs(args))
        {
            PrintUsageHelp();
            return null;
        } else {
            return new PassGenArgs()
            {
                Target = args.First(),
                Salt = args.Skip(1).SingleOrDefault(),
            };
        }
    }

    public void PrintUsageHelp()
    {
        Console.WriteLine("Usage: passgen <target> [salt]");
        Console.WriteLine("target   key of target website/program to generate password for");
        Console.WriteLine("salt     secret salt which is the password encrypted with");
    }

    public void PrintSaltHelp()
    {
        Console.WriteLine("Salt must be provided with one of the following ways: ");
        Console.WriteLine("1. Command line arguments: dotnet passgen <target> [salt]");
        Console.WriteLine($"2. Environment variable '{SaltResolver.PgSalt}'");
        if (SaltResolver.IsWindows)
            Console.WriteLine($"3. Windows CredentialManager's generic credential '{SaltResolver.PgSalt}'");
        if (SaltResolver.IsOSX)
            Console.WriteLine($"3. OSX keychain generic password '{SaltResolver.PgSalt}'");
        else if (SaltResolver.IsLinux)
            Console.WriteLine("3. Contents of passgen salt file in your home directory: ~/.passgen/salt");
    }

    private bool IsHelp(string[] args) => args.Any(it => it == "-h" || it == "--help");

    private bool AreValidArgs(string[] args)
    {
        return args.Length >= 1
            && args.Length <= 2
            && !args.Any(it => it == null)
            && !args.First().StartsWith("-");
    }
}
